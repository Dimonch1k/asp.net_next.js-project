using System;

namespace backend_c_.DTO.User;

public class RegisterDto
{
  public required string Username { get; set; }
  public required string Password { get; set; }
  public required string Email { get; set; }
  public required string FullName { get; set; }
  public required string TimeZoneId { get; set; }
}
