namespace backend_c_.DTO.User;

public class UserDto
{
  public int Id { get; set; }
  public required string Username { get; set; }
  public required string Email { get; set; }
  public required string FullName { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
  public string? CreatedAtFormatted { get; set; }
  public string? UpdatedAtFormatted {  get; set; }
  public List<string> Files { get; set; }
  public List<string> SharedFiles { get; set; }
}
