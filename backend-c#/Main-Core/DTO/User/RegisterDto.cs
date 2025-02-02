using System;

namespace backend_c_.DTO.User;

public class RegisterDto
{
  public string Username { get; set; }
  public string Password { get; set; }
  public string Email { get; set; }
  public string FullName { get; set; }
  public string TimeZoneId { get; set; }
}
