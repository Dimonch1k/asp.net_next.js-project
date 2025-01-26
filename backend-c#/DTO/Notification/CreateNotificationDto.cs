namespace backend_c_.DTO.Notification;

public class CreateNotificationDto
{
  public int UserId { get; set; }
  public string Message { get; set; } = string.Empty;
}
