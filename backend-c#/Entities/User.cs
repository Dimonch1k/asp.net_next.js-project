namespace backend_c_.Entity;

public class User
{
  private static readonly List<SharedFile> sharedFiles = new List<SharedFile>();

  public int Id { get; set; }
  public string Username { get; set; }
  public string PasswordHash { get; set; }
  public string Email { get; set; }
  public string FullName { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime UpdatedAt { get; set; } = DateTime.Now;

  public List<File> Files { get; set; }
  public List<AccessLog> AccessLogs { get; set; }
  public List<SharedFile> OwnedFiles { get; set; } = new();
  public List<SharedFile> SharedWithFiles { get; set; } = new();

}