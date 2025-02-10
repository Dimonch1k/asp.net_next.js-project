using backend_c_.DTO.Notification;

namespace backend_c_.Service;

public interface INotificationService
{
  IEnumerable<NotificationDto> GetUserNotifications( int userId );
  NotificationDto SendNotification( CreateNotificationDto notificationDto );
}
