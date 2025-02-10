using backend_c_.DTO.Notification;

namespace backend_c_.Service;

public interface INotificationService
{
  Task<IEnumerable<NotificationDto>> GetUserNotifications( int userId );
  Task<NotificationDto> SendNotification( CreateNotificationDto notificationDto );
  Task DeleteUserNotification( int notificationId );
  void DeleteUserNotifications (int userId );
}
