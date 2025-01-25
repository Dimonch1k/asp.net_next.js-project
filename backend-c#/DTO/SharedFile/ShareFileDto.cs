using backend_c_.Enums;

namespace backend_c_.DTO.SharedFile;

public class ShareFileDto
{
  public int Id { get; set; }
  public int FileId { get; set; }
  public int OwnerId { get; set; }
  public int SharedWithId { get; set; }
  public string Permission { get; set; }
}
