using backend_c_.Enums;

namespace backend_c_.DTO.SharedFile;

public class ShareFileDto
{
  public int Id { get; set; }
  public required int FileId { get; set; }
  public required int OwnerId { get; set; }
  public required int SharedWithId { get; set; }
  public required string Permission { get; set; }
}
