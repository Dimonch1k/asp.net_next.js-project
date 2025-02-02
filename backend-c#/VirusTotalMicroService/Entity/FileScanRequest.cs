namespace VirusTotalMicroService.Entity;

public class FileScanRequest
{
  public int Id { get; set; }
  public Guid FileId { get; set; }
  public string FilePath { get; set; }
  public string FileName { get; set; }
  public string Status { get; set; }
  public string? ScanResult { get; set; }
  public DateTime CreatedAt { get; set; }
}
