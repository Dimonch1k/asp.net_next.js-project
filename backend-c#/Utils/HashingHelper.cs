namespace backend_c_.Utils;

public class HashingHelper
{
  public static string HashPassword( string password )
  {
    return BCrypt.Net.BCrypt.HashPassword( password );
  }

  public static bool VerifyPassword( string password, string hashedPassword )
  {
    return BCrypt.Net.BCrypt.Verify( password, hashedPassword );
  }
}
