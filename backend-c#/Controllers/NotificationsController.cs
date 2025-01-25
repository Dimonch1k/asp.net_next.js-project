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
  public IActionResult GetUserNotifications( int id )
  {
    LoggingHelper.LogRequest( _logger, $"get user's notifications with ID: {id}" );

    IEnumerable<NotificationDto> notificationsDto = _notificationService.GetUserNotifications( id );

    if ( notificationsDto == null )
    {
      LoggingHelper.LogFailure( _logger, "No user's notifications found.", new { Id = id } );
    }
    LoggingHelper.LogSuccess( _logger, "Notifications received successfully", new { Id = id } );

    return Ok( notificationsDto );
  }
}
