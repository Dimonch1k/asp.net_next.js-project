using System.ComponentModel.DataAnnotations.Schema;

namespace backend_c_.Entity;

public class MediaFile
{
  public int Id { get; set; }
  public int UserId { get; set; }
  public required string FileName { get; set; }
  public required string FilePath { get; set; }
  public int FileSize { get; set; }
  public required string FileType { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
  public DateTime? DeletedAt { get; set; }

  [ForeignKey( "UserId" )]
  public User User { get; set; }
  public List<AccessLog> AccessLogs { get; set; } = new();
  public List<FileVersion> Versions { get; set; } = new();
  public List<SharedFile> SharedFiles { get; set; } = new();
}
