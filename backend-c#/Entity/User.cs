namespace backend_c_.Entity;

public class User
{
  public int Id { get; set; }
  public required string Username { get; set; }
  public required string PasswordHash { get; set; }
  public required string Email { get; set; }
  public required string FullName { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
  public required string TimeZoneId { get; set; }

  public List<MediaFile> Files { get; set; } = new();
  public List<AccessLog> AccessLogs { get; set; } = new();
  public List<SharedFile> OwnedFiles { get; set; } = new();
  public List<SharedFile> SharedWithFiles { get; set; } = new();
  public List<Notification> notifications { get; set; } = new();
}
