using backend_c_.DTO.Notification;
using backend_c_.Entity;

namespace backend_c_.Service.Impl;

public class NotificationService : INotificationService
{
  private readonly AppDbContext _dbContext;
  private readonly Lazy<IUserService> _userService;

  public NotificationService( AppDbContext dbContext, Lazy<IUserService> userService )
  {
    _dbContext = dbContext;
    _userService = userService;
  }
  public IEnumerable<NotificationDto> GetUserNotifications( int userId )
  {
    User? user = _userService.Value.GetUserIfExists( userId );

    return _dbContext.Notifications
      .Where( n => n.UserId == userId )
      .OrderByDescending( n => n.CreatedAt )
      .Select( n => NotificationToDto( n ) )
      .ToList();
  }

  public NotificationDto SendNotification( CreateNotificationDto notificationDto )
  {
    User? user = _userService.Value.GetUserIfExists( notificationDto.UserId );

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


  private NotificationDto NotificationToDto( Notification notification )
  {
    return new NotificationDto
    {
      Id = notification.Id,
      UserId = notification.UserId,
      Message = notification.Message,
      CreatedAt = notification.CreatedAt
    };
  }
}
