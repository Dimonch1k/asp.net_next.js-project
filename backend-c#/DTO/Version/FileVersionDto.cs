using backend_c_.Entity;

namespace backend_c_.DTO.Version;

public class FileVersionDto
{
  public int Id { get; set; }
  public int FileId { get; set; }
  public required string VersionName { get; set; }
  public required string VersionPath { get; set; }
}
