using System.ComponentModel.DataAnnotations.Schema;

namespace backend_c_.Entity;

public class FileVersion
{
  public int Id { get; set; }
  public int FileId { get; set; }
  public required string VersionName { get; set; }
  public required string VersionPath { get; set; }
  public DateTime CreatedAt { get; set; }

  [ForeignKey( "FileId" )]
  public MediaFile File { get; set; }
}
