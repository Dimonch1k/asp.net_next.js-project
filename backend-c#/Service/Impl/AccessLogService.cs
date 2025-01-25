using backend_c_.DTO.Access;
using backend_c_.DTO.Notification;
using backend_c_.Entity;
using backend_c_.Enums;
using backend_c_.Exceptions;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend_c_.Service.Impl;

public class AccessLogService : IAccessLogService
{
  private readonly AppDbContext _dbContext;
  private readonly INotificationService _notificationService;

  public AccessLogService( AppDbContext dbContext, INotificationService notificationService )
  {
    _dbContext = dbContext;
    _notificationService = notificationService;
  }

  public IEnumerable<AccessLogDto> FindAccessByFile( int fileId )
  {
    return _dbContext.AccessLogs
      .Where( log => log.FileId == fileId )
      .Select( AccessLogToDto )
      .ToList();
  }

  public IEnumerable<AccessLogDto> FindAccessByUser( int userId )
  {
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
    AccessLog? accessLog = _dbContext.AccessLogs.Find( id );

    if ( accessLog == null )
    {
      throw new InvalidOperationException( $"Access log with ID {id} not found." );
    }

    return AccessLogToDto( accessLog );
  }

  public AccessLogDto Create( CreateAccessLogDto createAccessLogDto )
  {
    MediaFile? file = _dbContext.Files
      .Include( f => f.User )
      .FirstOrDefault( f => f.Id == createAccessLogDto.FileId );

    if ( file == null )
    {
      throw new Exception( $"File with ID {createAccessLogDto.FileId} not found." );
    }

    SharedFile? sharedFile = _dbContext.SharedFiles
      .FirstOrDefault(
        sf => sf.FileId == createAccessLogDto.FileId
        && sf.SharedWithId == createAccessLogDto.UserId
      );

    if ( sharedFile == null )
    {
      throw new Exception( $"Shared file not found." );
    }

    if ( !IsAccessAllowed( sharedFile.Permission, createAccessLogDto.AccessType ) )
    {
      throw new Exception( "Access type isn't allowed." );
    }

    AccessLog newAccessLog = new AccessLog
    {
      FileId = createAccessLogDto.FileId,
      UserId = createAccessLogDto.UserId,
      AccessType = (AccessType) Enum.Parse( typeof( AccessType ), createAccessLogDto.AccessType.ToLower() ),
      AccessTime = DateTime.UtcNow
    };

    _dbContext.AccessLogs.Add( newAccessLog );
    _dbContext.SaveChanges();

    if ( file.UserId != createAccessLogDto.UserId )
    {
      string message = $"Your file '{file.FileName}' was {createAccessLogDto.AccessType.ToString().ToLower()} by user {createAccessLogDto.UserId}.";

      // Send notification to file owner if some action happened (open, write, download)
      _notificationService.SendNotification(
        new CreateNotificationDto
        {
          UserId = file.UserId,
          Message = message
        }
      );
    }

    return AccessLogToDto( newAccessLog );
  }

  public AccessLogDto Update( int id, UpdateAccessLogDto updateAccessLogDto )
  {
    AccessLog? accessLog = _dbContext.AccessLogs.Find( id );

    if ( accessLog == null )
    {
      throw new InvalidOperationException( $"Access log with ID {id} not found." );
    }

    accessLog.AccessType = (AccessType) Enum.Parse( typeof( AccessType ), updateAccessLogDto.AccessType.ToLower() );

    _dbContext.AccessLogs.Update( accessLog );
    _dbContext.SaveChanges();

    return AccessLogToDto( accessLog );
  }

  public AccessLogDto Remove( int id )
  {
    AccessLog? accessLog = _dbContext.AccessLogs.Find( id );

    if ( accessLog == null )
    {
      throw new InvalidOperationException( $"Access log with ID {id} not found." );
    }

    _dbContext.AccessLogs.Remove( accessLog );
    _dbContext.SaveChanges();

    return AccessLogToDto( accessLog );
  }


  private static AccessLogDto AccessLogToDto( AccessLog accessLog )
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

  private bool IsAccessAllowed( AccessType permission, string requestedAccess )
  {
    var reqAccess = (AccessType) Enum.Parse( typeof( AccessType ), requestedAccess.ToLower() );

    if ( ( permission == AccessType.read && reqAccess != AccessType.read )
      || ( permission == AccessType.write && reqAccess == AccessType.download ) )
    {
      return false;
    }
    return true;
  }
}
