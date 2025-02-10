namespace backend_c_.DTO.File;

public class FileDto
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
  public string? CreatedAtFormatted { get; set; }
  public string? UpdatedAtFormatted { get; set; }
}
