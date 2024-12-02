namespace backend_c_.DTO.File;

public class FileUploadDto
{
  public int UserId { get; set; }
  public string FileName { get; set; }
  public string FilePath { get; set; }
  public int FileSize { get; set; }
  public string FileType { get; set; }
}
