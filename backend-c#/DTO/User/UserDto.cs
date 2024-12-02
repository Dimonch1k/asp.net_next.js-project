namespace backend_c_.DTO.User;

public class UserDto
{
  public int Id { get; set; }
  public string Username { get; set; }
  public string Email { get; set; }
  public string FullName { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
  public List<string> Files { get; set; }
  public List<string> SharedFiles { get; set; }
}
