namespace backend_c_.DTO.Access;

public class AccessLogDto
{
  public int Id { get; set; }
  public int FileId { get; set; }
  public int UserId { get; set; }
  public string AccessType { get; set; }
  public DateTime AccessTime { get; set; }
}
