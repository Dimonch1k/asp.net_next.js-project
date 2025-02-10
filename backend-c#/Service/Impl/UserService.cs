using backend_c_.Controllers;
using backend_c_.DTO.User;
using backend_c_.DTO.Version;
using backend_c_.Entity;
using backend_c_.Enums;
using backend_c_.Exceptions;
using backend_c_.Utilities;
using backend_c_.Utils;
using Microsoft.EntityFrameworkCore;

namespace backend_c_.Service.Impl;

public class UserService : IUserService
{
  private AppDbContext _dbContext;
  private readonly TimeZoneHelper _timeZoneHelper;
  private readonly ILogger<UserService> _logger;

  public UserService( AppDbContext dbContext, TimeZoneHelper timeZoneHelper, ILogger<UserService> logger )
  {
    _dbContext = dbContext;
    _timeZoneHelper = timeZoneHelper;
    _logger = logger;
  }

  public List<UserDto> GetAllUsers( )
  {
    return _dbContext.Users
      .Select( UserToDto )
      .ToList();
  }

  public async Task<UserDto> GetUserById( int id )
  {
    User user = await GetUserIfExists( id );

    return UserToDto( user );
  }

  public async Task<UserDto> UpdateUser( int id, UpdateUserDto data )
  {
    User user = await GetUserIfExists( id );

    user.Username = data.Username;
    user.Email = data.Email;
    user.FullName = data.FullName;
    user.UpdatedAt = DateTime.UtcNow;

    _dbContext.Users.Update( user );
    _dbContext.SaveChanges();

    return UserToDto( user );
  }

  public async Task<UserDto> DeleteUser( int id )
  {
    User user = await GetUserIfExists( id );

    _dbContext.Users.Remove( user );
    _dbContext.SaveChanges();

    return UserToDto( user );
  }

  public async Task<User> RegisterUser( RegisterDto registerDto )
  {
    await EnsureUserIsUnique( registerDto.Username, registerDto.Email );

    string hashedPassword = HashingHelper.HashPassword( registerDto.Password );

    User newUser = new User
    {
      Username = registerDto.Username,
      PasswordHash = hashedPassword,
      Email = registerDto.Email,
      FullName = registerDto.FullName,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow,
      TimeZoneId = registerDto.TimeZoneId
    };

    _dbContext.Users.Add( newUser );
    await _dbContext.SaveChangesAsync();

    return newUser;
  }


  public async Task<User> GetUserIfExists( int? id )
  {
    User? user = await _dbContext.Users.FindAsync( id );

    if ( user == null )
    {
      _logger.LogError( "User not found" );

      throw new ServerException( $"User with ID='{id}' not found", ExceptionStatusCode.UserNotFound );
    }

    return user;
  }

  public async Task<User> GetUserByUsernameIfExists( string username )
  {
    User? user = await _dbContext.Users.FirstOrDefaultAsync( u => u.Username == username );

    if ( user == null )
    {
      _logger.LogError( "User not found" );

      throw new ServerException( $"User with Username='{username}' not found", ExceptionStatusCode.UserNotFound );
    }

    return user;
  }

  public async Task EnsureUserExists( int? id )
  {
    if ( await _dbContext.Users.FindAsync( id ) == null )
    {
      _logger.LogError( "User not found" );

      throw new ServerException( $"User with ID='{id}' not found", ExceptionStatusCode.UserNotFound );
    }
  }

  public async Task EnsureUserIsUnique( string username, string email )
  {
    User? user = await _dbContext.Users.FirstOrDefaultAsync( user =>
      user.Username == username
      || user.Email == email
    );

    if ( user != null )
    {
      _logger.LogError( "Username or Email already exists" );

      throw new ServerException( "Username or Email already exists", ExceptionStatusCode.UserDuplicate );
    }
  }

  private UserDto UserToDto( User user )
  {
    return new UserDto
    {
      Id = user.Id,
      Username = user.Username,
      Email = user.Email,
      FullName = user.FullName,
      CreatedAt = user.CreatedAt,
      UpdatedAt = user.UpdatedAt,
      CreatedAtFormatted = _timeZoneHelper.GetHumanReadableTime( user.CreatedAt, user.TimeZoneId ),
      UpdatedAtFormatted = _timeZoneHelper.GetHumanReadableTime( user.UpdatedAt, user.TimeZoneId )
    };
  }

  public string GetUserTimeZone( int? id )
  {
    return _dbContext.Users.Find( id ).TimeZoneId;
  }
}
