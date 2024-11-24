namespace backend_c_.Entity;

public class Version
{
  public int Id { get; set; }
  public int FileId { get; set; }
  public string VersionName { get; set; }
  public string VersionPath { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.Now;

  public File File { get; set; }
}
