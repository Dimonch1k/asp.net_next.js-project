namespace backend_c_.DTO.File;

public class UploadFileDto
{
  public int UserId { get; set; }
  public string FileName { get; set; }
  public string FilePath { get; set; }
  public byte[] FileData { get; set; }
  public string FileType { get; set; }
}
