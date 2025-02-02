namespace backend_c_.Utilities;

public class TimeZoneHelper
{
  public static DateTime ConvertToUserTimeZone( DateTime utcDateTime, string timeZoneId )
  {
    try
    {
      var timeZone = TimeZoneInfo.FindSystemTimeZoneById( timeZoneId );
      return TimeZoneInfo.ConvertTimeFromUtc( utcDateTime, timeZone );
    }
    catch ( TimeZoneNotFoundException )
    {
      return utcDateTime;
    }
    catch ( InvalidTimeZoneException )
    {
      return utcDateTime;
    }
  }

  public static bool IsValidTimeZoneId( string timeZoneId )
  {
    return TimeZoneInfo.GetSystemTimeZones().Any( tz => tz.Id == timeZoneId );
  }
}
