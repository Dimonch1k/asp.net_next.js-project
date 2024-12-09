using System.Linq;

namespace backend_c_.Utils;

public static class ValidationHelpers
{
  public static bool BeAValidFileType( string fileType )
  {
    string[] allowedFileTypes = { "image/jpeg", "image/jpg", "image/png" };
    return allowedFileTypes.Contains( fileType );
  }

  public static bool BeAValidTimeZone( string timeZoneId )
  {
    return TimeZoneInfo.GetSystemTimeZones().Any( tz => tz.Id == timeZoneId );
  }

  public static bool BeAValidAccessType( string accessType )
  {
    string[] allowedAccessTypes = { "read", "write", "delete" };
    return allowedAccessTypes.Contains( accessType );
  }
}
