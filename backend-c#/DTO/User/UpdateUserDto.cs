namespace backend_c_.DTO.User;

public class UpdateUserDto
{
  public required string Username { get; set; }
  public required string Email { get; set; }
  public required string FullName { get; set; }
}
