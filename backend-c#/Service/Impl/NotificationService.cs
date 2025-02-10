using backend_c_.DTO.Notification;
using backend_c_.DTO.SharedFile;
using backend_c_.Entity;
using backend_c_.Enums;
using backend_c_.Exceptions;
using backend_c_.Utilities;

namespace backend_c_.Service.Impl;

public class NotificationService : INotificationService
{
  private readonly AppDbContext _dbContext;
  private readonly Lazy<IUserService> _userService;
  private readonly TimeZoneHelper _timeZoneHelper;
  private readonly ILogger<NotificationService> _logger;

  public NotificationService( AppDbContext dbContext, Lazy<IUserService> userService, TimeZoneHelper timeZoneHelper, ILogger<NotificationService> logger )
  {
    _dbContext = dbContext;
    _userService = userService;
    _timeZoneHelper = timeZoneHelper;
    _logger = logger;
  }
  public async Task<IEnumerable<NotificationDto>> GetUserNotifications( int userId )
  {
    User? user = await _userService.Value.GetUserIfExists( userId );

    return _dbContext.Notifications
      .Where( n => n.UserId == userId )
      .OrderByDescending( n => n.CreatedAt )
      .Select( NotificationToDto )
      .ToList();
  }

  public async Task<NotificationDto> SendNotification( CreateNotificationDto notificationDto )
  {
    User? user = await _userService.Value.GetUserIfExists( notificationDto.UserId );

    Notification notification = new Notification
    {
      UserId = notificationDto.UserId,
      Message = notificationDto.Message,
      CreatedAt = DateTime.UtcNow
    };

    _dbContext.Notifications.Add( notification );
    _dbContext.SaveChanges();

    return NotificationToDto( notification );
  }

  public async Task DeleteUserNotification( int notificationId )
  {
    Notification? notification = await GetNotificationIfExists( notificationId );

    _dbContext.Notifications.Remove( notification );
    _dbContext.SaveChanges();
  }

  public void DeleteUserNotifications( int userId )
  {
    List<Notification> notifications = _dbContext.Notifications
      .Where( n => n.UserId == userId )
      .ToList();

    if ( notifications.Count > 0 )
    {
      _dbContext.Notifications.RemoveRange( notifications );
      _dbContext.SaveChanges();
    }
  }


  private async Task<Notification> GetNotificationIfExists( int notificationId )
  {
    Notification? notification = await _dbContext.Notifications.FindAsync( notificationId );

    if ( notification == null )
    {
      _logger.LogError( "Notification not found" );

      throw new ServerException( $"Notification with ID='{notificationId}' not found", ExceptionStatusCode.NotificationNotFound );
    }

    return notification;
  }

  private NotificationDto NotificationToDto( Notification notification )
  {
    User? user = _dbContext.Users.Find( notification.UserId );

    return new NotificationDto
    {
      Id = notification.Id,
      UserId = notification.UserId,
      Message = notification.Message,
      CreatedAt = notification.CreatedAt,
      CreatedAtFormatted = _timeZoneHelper.GetHumanReadableTime( notification.CreatedAt, user.TimeZoneId )
    };
  }
}
