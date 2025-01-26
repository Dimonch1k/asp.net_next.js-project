namespace backend_c_.Entity;

public class FileVersion
{
  public int Id { get; set; }
  public int FileId { get; set; }
  public string VersionName { get; set; }
  public string VersionPath { get; set; }
  public DateTime CreatedAt { get; set; }

  public MediaFile File { get; set; }
}
