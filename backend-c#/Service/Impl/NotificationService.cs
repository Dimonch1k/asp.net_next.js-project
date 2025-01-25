using backend_c_.DTO.Notification;
using backend_c_.Entity;

namespace backend_c_.Service.Impl;

public class NotificationService : INotificationService
{
  private readonly AppDbContext _dbContext;

  public NotificationService( AppDbContext dbContext )
  {
    _dbContext = dbContext;
  }

  public NotificationDto SendNotification( CreateNotificationDto notificationDto )
  {
    User? user = _dbContext.Users.FirstOrDefault( u => u.Id == notificationDto.UserId );

    if ( user == null )
    {
      throw new Exception( $"User with ID {notificationDto.UserId} not found" );
    }

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

  public IEnumerable<NotificationDto> GetUserNotifications( int userId )
  {
    User? user = _dbContext.Users.FirstOrDefault( u => u.Id == userId );

    if ( user == null )
    {
      throw new Exception( $"User with ID {userId} not found" );
    }

    return _dbContext.Notifications
      .Where( n => n.UserId == userId )
      .OrderByDescending( n => n.CreatedAt )
      .Select( n => NotificationToDto( n ) )
      .ToList();
  }

  private static NotificationDto NotificationToDto( Notification notification )
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
