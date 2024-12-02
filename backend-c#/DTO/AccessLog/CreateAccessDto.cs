namespace backend_c_.DTO.Access;

public class CreateAccessDto
{
  public int FileId { get; set; }
  public int UserId { get; set; }
  public string AccessType { get; set; }
}
