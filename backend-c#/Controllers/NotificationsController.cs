using backend_c_.DTO.Notification;
using backend_c_.Service;
using backend_c_.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_c_.Controllers;

[ApiController]
[Route( "api/v1/[controller]" )]
[Authorize]
public class NotificationsController : ControllerBase
{
  private readonly INotificationService _notificationService;
  private readonly ILogger<UsersController> _logger;

  public NotificationsController( INotificationService notificationService, ILogger<UsersController> logger )
  {
    _notificationService = notificationService;
    _logger = logger;
  }

  [HttpGet( "{id}" )]
  public async Task<IActionResult> GetUserNotifications( int id )
  {
    _logger.LogInformation( $"Received request to get user's notifications with ID: {id}" );

    IEnumerable<NotificationDto> notificationsDto = await _notificationService.GetUserNotifications( id );

    _logger.LogInformation( "Notifications received successfully" );

    return Ok( notificationsDto );
  }

  [HttpDelete( "{notificationId}" )]
  public async Task<IActionResult> DeleteUserNotification( int notificationId )
  {
    _logger.LogInformation( $"Received request to delete user's notification with ID: {notificationId}" );

    await _notificationService.DeleteUserNotification( notificationId );

    _logger.LogInformation( "Notifications removed successfully" );

    return Ok( "Notification deleted successfully" );
  }

  [HttpDelete( "delete-all/{userId}" )]
  public IActionResult DeleteAllUserNotifications( int userId )
  {
    _logger.LogInformation( $"Received request to delete all notifications for user with ID: {userId}" );

    _notificationService.DeleteUserNotifications( userId );

    _logger.LogInformation( "All notifications deleted successfully" );

    return Ok( "All notifications deleted successfully" );
  }
}
