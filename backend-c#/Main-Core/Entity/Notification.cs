using backend_c_.Entity;

namespace backend_c_.Entity;

public class Notification
{
  public int Id { get; set; }
  public int UserId { get; set; }
  public string Message { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; }

  public User User { get; set; }
}
