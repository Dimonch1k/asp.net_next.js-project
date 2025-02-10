using backend_c_.Entity;
using backend_c_.Exceptions;
using backend_c_.Service;

namespace backend_c_.Utilities;

public class TimeZoneHelper
{
  private readonly AppDbContext _dbContext;
  private readonly Lazy<IUserService> _userService;

  public TimeZoneHelper( AppDbContext dbContext, Lazy<IUserService> userService )
  {
    _dbContext = dbContext;
    _userService = userService;
  }

  public DateTime ConvertToUserTimeZone( DateTime utcDateTime, string timeZoneId )
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

  public bool IsValidTimeZoneId( string timeZoneId )
  {
    return TimeZoneInfo.GetSystemTimeZones().Any( tz => tz.Id == timeZoneId );
  }

  public string GetHumanReadableTime( DateTime time, string timeZoneId )
  {
    return ConvertToUserTimeZone( time, timeZoneId ).ToString( "g" );
  }

  //public string GetHumanReadableTimeByUserId( DateTime time, string timeZoneId )
  //{
  //  return ConvertToUserTimeZone( time, timeZoneId ).ToString( "g" );
  //}
}
