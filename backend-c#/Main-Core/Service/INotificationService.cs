using backend_c_.DTO.Notification;

namespace backend_c_.Service;

public interface INotificationService
{
  NotificationDto SendNotification( CreateNotificationDto notificationDto );
  IEnumerable<NotificationDto> GetUserNotifications( int userId );
}
