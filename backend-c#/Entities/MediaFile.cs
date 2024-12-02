namespace backend_c_.Entity;

public class MediaFile
{
  public int Id { get; set; }
  public int UserId { get; set; }
  public string FileName { get; set; }
  public string FilePath { get; set; }
  public int FileSize { get; set; }
  public string FileType { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
  public DateTime? DeletedAt { get; set; }

  public User User { get; set; }
  public List<AccessLog> AccessLogs { get; set; }
  public List<FileVersion> Versions { get; set; }
  public List<SharedFile> SharedFiles { get; set; } = new();
}
