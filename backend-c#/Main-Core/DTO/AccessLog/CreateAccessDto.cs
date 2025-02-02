using backend_c_.Enums;

namespace backend_c_.DTO.Access;

public class CreateAccessLogDto
{
  public int FileId { get; set; }
  public int UserId { get; set; }
  public string AccessType { get; set; }
}
