namespace backend_c_.DTO.Access;

public class AccessLogDto
{
  public int Id { get; set; }
  public int SharedFileId { get; set; }
  public int UserId { get; set; }
  public required string AccessType { get; set; }
  public DateTime AccessTime { get; set; }
  public string? AccessTimeFormatted { get; set; }
}
