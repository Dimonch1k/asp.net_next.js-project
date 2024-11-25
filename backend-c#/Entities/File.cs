namespace backend_c_.Entity;

public class File
{
  public int Id { get; set; }
  public int UserId { get; set; }
  public string FileName { get; set; }
  public string FilePath { get; set; }
  public int FileSize { get; set; }
  public string FileType { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime UpdatedAt { get; set; } = DateTime.Now;
  public DateTime? DeletedAt { get; set; }

  public User User { get; set; }
  public List<AccessLog> AccessLogs { get; set; }
  public List<Version> Versions { get; set; }
  public List<SharedFile> SharedFiles { get; set; } = new();
}
