namespace backend_c_.DTO.Version;

public class CreateFileVersionDto
{
  public int FileId { get; set; }
  public required string VersionName { get; set; }
}
