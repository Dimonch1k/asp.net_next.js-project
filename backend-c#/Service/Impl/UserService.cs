using backend_c_.Controllers;
using backend_c_.DTO.User;
using backend_c_.DTO.Version;
using backend_c_.Entity;
using backend_c_.Enums;
using backend_c_.Exceptions;
using backend_c_.Utilities;
using backend_c_.Utils;

namespace backend_c_.Service.Impl;

public class UserService : IUserService
{
  private AppDbContext _dbContext;
  private readonly ILogger<UserService> _logger;

  public UserService( AppDbContext dbContext, ILogger<UserService> logger )
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public List<UserDto> GetAllUsers( )
  {
    return _dbContext.Users
      .Select( user => UserToDto( user ) )
      .ToList();
  }

  public UserDto GetUserById( int id )
  {
    User user = GetUserIfExists( id );

    return UserToDto( user );
  }

  public UserDto UpdateUser( int id, UpdateUserDto data )
  {
    User user = GetUserIfExists( id );

    user.Username = data.Username;
    user.Email = data.Email;
    user.FullName = data.FullName;

    _dbContext.SaveChanges();

    return new UserDto
    {
      Id = user.Id,
      Username = user.Username,
      Email = user.Email,
      FullName = data.FullName
    };
  }

  public UserDto DeleteUser( int id )
  {
    User user = GetUserIfExists( id );

    _dbContext.Users.Remove( user );
    _dbContext.SaveChanges();

    return UserToDto( user );
  }

  public User RegisterUser( RegisterDto registerDto )
  {
    EnsureUserIsUnique( registerDto.Username, registerDto.Email );

    string hashedPassword = HashingHelper.HashPassword( registerDto.Password );

    DateTime utcNow = DateTime.UtcNow;

    User newUser = new User
    {
      Username = registerDto.Username,
      PasswordHash = hashedPassword,
      Email = registerDto.Email,
      FullName = registerDto.FullName,
      CreatedAt = utcNow,
      UpdatedAt = utcNow,
      TimeZoneId = registerDto.TimeZoneId
    };

    _dbContext.Users.Add( newUser );
    _dbContext.SaveChanges();

    return newUser;
  }


  public User GetUserIfExists( int id )
  {
    User? user = _dbContext.Users.Find( id );

    if ( user == null )
    {
      _logger.LogError( "User not found" );

      throw new ServerException( $"User with ID='{id}' not found", ExceptionStatusCode.UserNotFound );
    }
    return user;
  }

  public void EnsureUserExists( int id )
  {
    if ( _dbContext.Users.Find( id ) == null )
    {
      _logger.LogError( "User not found" );

      throw new ServerException( $"User with ID='{id}' not found", ExceptionStatusCode.UserNotFound );
    }
  }

  public void EnsureUserIsNotNull( User? user )
  {
    if ( user == null )
    {
      _logger.LogError( "User not found" );

      throw new ServerException( $"User not found", ExceptionStatusCode.UserNotFound );
    }
  }

  public void EnsureUserIsUnique( string username, string email )
  {
    User? user = _dbContext.Users.FirstOrDefault( user =>
      user.Username == username
      && user.Email == email
    );

    if ( user != null )
    {
      _logger.LogError( "Username or email already exists" );

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
    };
  }
}
