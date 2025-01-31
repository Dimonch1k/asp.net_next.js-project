using backend_c_.DTO.Access;
using backend_c_.DTO.Notification;
using backend_c_.Entity;
using backend_c_.Enums;
using backend_c_.Exceptions;
using backend_c_.Utilities;
using Microsoft.EntityFrameworkCore;
using Npgsql.PostgresTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend_c_.Service.Impl;

public class AccessLogService : IAccessLogService
{
  private readonly AppDbContext _dbContext;
  private readonly IUserService _userService;
  private readonly IFileService _fileService;
  private readonly ISharedFileService _sharedFileService;
  private readonly INotificationService _notificationService;
  private readonly ILogger<AccessLogService> _logger;

  public AccessLogService( AppDbContext dbContext, IUserService userService, IFileService fileService, ISharedFileService sharedFileService, INotificationService notificationService, Logger<AccessLogService> logger )
  {
    _dbContext = dbContext;
    _userService = userService;
    _fileService = fileService;
    _sharedFileService = sharedFileService;
    _notificationService = notificationService;
    _logger = logger;
  }

  public IEnumerable<AccessLogDto> FindAccessByFile( int fileId )
  {
    _fileService.CheckIfFileExists( fileId );

    return _dbContext.AccessLogs
      .Where( log => log.FileId == fileId )
      .Select( AccessLogToDto )
      .ToList();
  }

  public IEnumerable<AccessLogDto> FindAccessByUser( int userId )
  {
    _userService.CheckIfUserExists( userId );

    return _dbContext.AccessLogs
      .Where( log => log.UserId == userId )
      .Select( AccessLogToDto )
      .ToList();
  }

  public IEnumerable<AccessLogDto> FindAll( )
  {
    return _dbContext.AccessLogs
      .Select( AccessLogToDto )
      .ToList();
  }

  public AccessLogDto FindOne( int id )
  {
    AccessLog? accessLog = CheckIfAccessLogExists( id );

    return AccessLogToDto( accessLog );
  }

  public AccessLogDto Create( CreateAccessLogDto createAccessLogDto )
  {
    MediaFile? file = _dbContext.Files
      .Include( f => f.User )
      .FirstOrDefault( f => f.Id == createAccessLogDto.FileId );

    _fileService.CheckIfFileIsNull( file );

    CheckIfUserIdMatch( file?.UserId, createAccessLogDto.UserId );

    SharedFile? sharedFile = _dbContext.SharedFiles
    .FirstOrDefault(
      sf => sf.FileId == createAccessLogDto.FileId
      && sf.SharedWithId == createAccessLogDto.UserId
    );

    _sharedFileService.CheckIfSharedFileIsNull( sharedFile );

    IsAccessAllowed( sharedFile?.Permission, createAccessLogDto.AccessType );

    AccessLog newAccessLog = new AccessLog
    {
      FileId = createAccessLogDto.FileId,
      UserId = createAccessLogDto.UserId,
      AccessType = (AccessType) Enum.Parse( typeof( AccessType ), createAccessLogDto.AccessType.ToLower() ),
      AccessTime = DateTime.UtcNow
    };

    _dbContext.AccessLogs.Add( newAccessLog );
    _dbContext.SaveChanges();

    // Send notification to file owner if some action happened (open, write, download)
    SendNotification( file, createAccessLogDto );

    return AccessLogToDto( newAccessLog );
  }

  public AccessLogDto Update( int id, UpdateAccessLogDto updateAccessLogDto )
  {
    AccessLog? accessLog = CheckIfAccessLogExists( id );

    accessLog.AccessType = (AccessType) Enum.Parse( typeof( AccessType ), updateAccessLogDto.AccessType.ToLower() );

    _dbContext.AccessLogs.Update( accessLog );
    _dbContext.SaveChanges();

    return AccessLogToDto( accessLog );
  }

  public AccessLogDto Remove( int id )
  {
    AccessLog? accessLog = CheckIfAccessLogExists( id );

    _dbContext.AccessLogs.Remove( accessLog );
    _dbContext.SaveChanges();

    return AccessLogToDto( accessLog );
  }


  public AccessLog CheckIfAccessLogExists( int accessLogId )
  {
    AccessLog? accessLog = _dbContext.AccessLogs.Find( accessLogId );

    if ( accessLog == null )
    {
      LoggingHelper.LogFailure( _logger, "Access Log not found", new { Id = accessLogId } );

      throw new ServerException( $"Access Log with ID='{accessLogId}' not found.", ExceptionStatusCode.AccessLogNotFound );
    }

    return accessLog;
  }

  private AccessLogDto AccessLogToDto( AccessLog accessLog )
  {
    return new AccessLogDto
    {
      Id = accessLog.Id,
      FileId = accessLog.FileId,
      UserId = accessLog.UserId,
      AccessType = accessLog.AccessType.ToString(),
      AccessTime = accessLog.AccessTime
    };
  }

  private void SendNotification( MediaFile? file, CreateAccessLogDto dto )
  {
    string message = $"Your file '{file?.FileName}' was {dto.AccessType.ToString().ToLower()} by user {dto.UserId}.";

    _notificationService.SendNotification(
      new CreateNotificationDto
      {
        UserId = file.UserId,
        Message = message
      }
    );
  }

  private void IsAccessAllowed( AccessType? permission, string requestedAccess )
  {
    AccessType reqAccess = (AccessType) Enum.Parse( typeof( AccessType ), requestedAccess.ToLower() );

    if ( ( permission == AccessType.read && reqAccess != AccessType.read )
      || ( permission == AccessType.write && reqAccess == AccessType.download ) )
    {
      LoggingHelper.LogFailure( _logger, "You haven't access to this file!" );

      throw new ServerException( "You haven't access to this file!", ExceptionStatusCode.FileNotAccessible );
    }
  }

  private void CheckIfUserIdMatch( int? fileUserId, int accessLogUserId )
  {
    if ( fileUserId != accessLogUserId )
    {
      LoggingHelper.LogFailure( _logger, $"User ID='{fileUserId}' and Access Log User ID='{accessLogUserId}' don't match" );

      throw new ServerException( $"User ID='{fileUserId}' and Access Log User ID='{accessLogUserId}' don't match", ExceptionStatusCode.BadRequest );
    }
  }
}
