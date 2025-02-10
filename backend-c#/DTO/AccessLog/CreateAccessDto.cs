using backend_c_.Enums;

namespace backend_c_.DTO.Access;

public class CreateAccessLogDto
{
  public int UserId { get; set; }
  public int SharedFileId { get; set; }
  public required string AccessType { get; set; }
}
