namespace backend_c_.Entity;

public class FileScanRequest
{
  public int Id { get; set; }
  public Guid FileId { get; set; }
  public required string FilePath { get; set; }
  public required string FileName { get; set; }
  public required string Status { get; set; }
  public string? ScanResult { get; set; }
  public DateTime CreatedAt { get; set; }
}
