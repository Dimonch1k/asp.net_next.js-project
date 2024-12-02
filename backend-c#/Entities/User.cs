namespace backend_c_.Entity;

public class User
{
  public int Id { get; set; }
  public string Username { get; set; }
  public string PasswordHash { get; set; }
  public string Email { get; set; }
  public string FullName { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  public List<MediaFile> Files { get; set; }
  public List<AccessLog> AccessLogs { get; set; }
  public List<SharedFile> OwnedFiles { get; set; } = new();
  public List<SharedFile> SharedWithFiles { get; set; } = new();
}
