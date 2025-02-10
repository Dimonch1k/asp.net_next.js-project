using System;
using backend_c_.DTO.Access;
using backend_c_.DTO.Notification;
using backend_c_.Entity;
using backend_c_.Enums;
using backend_c_.Exceptions;
using backend_c_.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Npgsql.PostgresTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend_c_.Service.Impl;

public class AccessLogService : IAccessLogService
{
  private readonly AppDbContext _dbContext;
  private readonly Lazy<IUserService> _userService;
  private readonly Lazy<IFileService> _fileService;
  private readonly Lazy<ISharedFileService> _sharedFileService;
  private readonly Lazy<INotificationService> _notificationService;
  private readonly ILogger<AccessLogService> _logger;
  private readonly TimeZoneHelper _timeZoneHelper;

  public AccessLogService( AppDbContext dbContext, Lazy<IUserService> userService, Lazy<IFileService> fileService, Lazy<ISharedFileService> sharedFileService, Lazy<INotificationService> notificationService, ILogger<AccessLogService> logger, TimeZoneHelper timeZoneHelper )
  {
    _dbContext = dbContext;
    _userService = userService;
    _fileService = fileService;
    _sharedFileService = sharedFileService;
    _notificationService = notificationService;
    _logger = logger;
    _timeZoneHelper = timeZoneHelper;
  }

  public IEnumerable<AccessLogDto> GetAllAccessLogs( )
  {
    return _dbContext.AccessLogs
      .Select( AccessLogToDto )
      .ToList();
  }

  public async Task<AccessLogDto> GetAccessLogById( int id )
  {
    AccessLog? accessLog = await GetAccessLogIfExists( id );

    return AccessLogToDto( accessLog );
  }

  public async Task<IEnumerable<AccessLogDto>> GetAccessLogsBySharedFileId( int sharedFileId )
  {
    SharedFile? sharedFile = await _sharedFileService.Value.GetSharedFileIfExists( sharedFileId );

    return _dbContext.AccessLogs
      .Where( log => log.SharedFileId == sharedFileId )
      .Select( AccessLogToDto )
      .ToList();
  }

  public async Task<IEnumerable<AccessLogDto>> GetAccessLogsByUserId( int userId )
  {
    await _userService.Value.EnsureUserExists( userId );

    return _dbContext.AccessLogs
      .Where( log => log.UserId == userId )
      .Select( AccessLogToDto )
      .ToList();
  }

  public async Task<AccessLogDto> CreateAccessLog( CreateAccessLogDto createAccessLogDto )
  {
    // When someone accessed the file (read - opened; write - changed smth. in file; download)
    SharedFile? sharedFile = await _sharedFileService.Value.GetSharedFileIfExists( createAccessLogDto.SharedFileId );

    EnsureUserIsNotSharedFileOwner( createAccessLogDto.UserId, sharedFile.OwnerId );

    await _userService.Value.EnsureUserExists( createAccessLogDto.UserId );

    MediaFile? file = await _fileService.Value.GetFileIfExists( sharedFile.FileId );

    IsAccessAllowed( sharedFile.Permission, createAccessLogDto.AccessType );

    AccessLog newAccessLog = new AccessLog
    {
      SharedFileId = sharedFile.Id,
      UserId = createAccessLogDto.UserId,
      AccessType = (AccessType) Enum.Parse( typeof( AccessType ), createAccessLogDto.AccessType.ToLower() ),
      AccessTime = DateTime.UtcNow
    };

    _dbContext.AccessLogs.Add( newAccessLog );
    _dbContext.SaveChanges();

    // Send notification to file owner if some action happened (open, write, download)
    await SendNotification( sharedFile, createAccessLogDto, file.FileName );

    await _userService.Value.EnsureUserExists( createAccessLogDto.UserId );

    return AccessLogToDto( newAccessLog );
  }

  public async Task<AccessLogDto> UpdateAccessLog( int id, UpdateAccessLogDto updateAccessLogDto )
  {
    AccessLog? accessLog = await GetAccessLogIfExists( id );

    accessLog.AccessType = (AccessType) Enum.Parse( typeof( AccessType ), updateAccessLogDto.AccessType.ToLower() );

    _dbContext.AccessLogs.Update( accessLog );
    _dbContext.SaveChanges();

    return AccessLogToDto( accessLog );
  }

  public async Task<AccessLogDto> DeleteAccessLog( int id )
  {
    AccessLog? accessLog = await GetAccessLogIfExists( id );

    _dbContext.AccessLogs.Remove( accessLog );
    _dbContext.SaveChanges();

    return AccessLogToDto( accessLog );
  }


  private async Task<AccessLog> GetAccessLogIfExists( int accessLogId )
  {
    AccessLog? accessLog = await _dbContext.AccessLogs.FindAsync( accessLogId );

    if ( accessLog == null )
    {
      _logger.LogInformation( "Access Log not found" );

      throw new ServerException( $"Access Log with ID='{accessLogId}' not found.", ExceptionStatusCode.AccessLogNotFound );
    }

    return accessLog;
  }

  private void EnsureUserIsNotSharedFileOwner( int userId, int ownerId )
  {
    if ( userId == ownerId )
    {
      _logger.LogInformation( "Access Log can't be created if owner of shared file accesses it." );

      throw new ServerException( "Access Log can't be created if owner of shared file accesses it.", ExceptionStatusCode.BadRequest );
    }
  }

  private async Task SendNotification( SharedFile sharedFile, CreateAccessLogDto dto, string fileName )
  {
    string message = $"Your file '{fileName}' was {dto.AccessType.ToString().ToLower()} by user {dto.UserId}.";

    await _notificationService.Value.SendNotification(
      new CreateNotificationDto
      {
        UserId = sharedFile.OwnerId,
        Message = message
      }
    );
  }

  private void IsAccessAllowed( AccessType permission, string requestedAccess )
  {
    AccessType reqAccess = (AccessType) Enum.Parse( typeof( AccessType ), requestedAccess.ToLower() );

    if ( ( permission == AccessType.read && reqAccess != AccessType.read )
      || ( permission == AccessType.write && reqAccess == AccessType.download ) )
    {
      _logger.LogInformation( "You haven't propper permission to this file!" );

      throw new ServerException( "You haven't propper permission to this file!", ExceptionStatusCode.FileNotAccessible );
    }
  }

  private static AccessLogDto AccessLogToDto( AccessLog accessLog )
  {
    return new AccessLogDto
    {
      Id = accessLog.Id,
      SharedFileId = accessLog.SharedFileId,
      UserId = accessLog.UserId,
      AccessType = accessLog.AccessType.ToString(),
      AccessTime = accessLog.AccessTime,
    };
  }
}
