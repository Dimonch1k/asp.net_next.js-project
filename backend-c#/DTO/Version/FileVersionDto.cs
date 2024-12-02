using backend_c_.Entity;

namespace backend_c_.DTO.Version;

public class FileVersionDto
{
  public int Id { get; set; }
  public int FileId { get; set; }
  public string VersionName { get; set; }
  public string VersionPath { get; set; }
}
