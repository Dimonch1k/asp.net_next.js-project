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

  public List<UserDto> FindAll( )
  {
    return _dbContext.Users
      .Select( user => UserToDto( user ) )
      .ToList();
  }

  public UserDto FindOne( int id )
  {
    User user = CheckIfUserExists( id );

    return UserToDto( user );
  }

  public UserDto Update( int id, UpdateUserDto data )
  {
    User user = CheckIfUserExists( id );

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

  public UserDto Remove( int id )
  {
    User user = CheckIfUserExists( id );

    _dbContext.Users.Remove( user );
    _dbContext.SaveChanges();

    return UserToDto( user );
  }

  public User Register( RegisterDto registerDto )
  {
    if ( UserExists( registerDto.Username, registerDto.Email ) )
    {
      LoggingHelper.LogFailure( _logger, "Username or email already exists", registerDto );

      throw new ServerException( "Username or Email already exists", ExceptionStatusCode.UserDuplicate );
    }

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


  public bool UserExists( string username, string email )
  {
    return _dbContext.Users.Any( user =>
      user.Username == username
      && user.Email == email
    );
  }

  public User CheckIfUserExists( int id )
  {
    User? user = _dbContext.Users.Find( id );

    if ( user == null )
    {
      LoggingHelper.LogFailure( _logger, "User not found", new { Id = id } );

      throw new ServerException( $"User with ID='{id}' not found", ExceptionStatusCode.UserNotFound );
    }
    return user;
  }

  public void CheckIfUserIsNull( User? user )
  {
    if ( user == null )
    {
      LoggingHelper.LogFailure( _logger, "User not found" );

      throw new ServerException( $"User not found", ExceptionStatusCode.UserNotFound );
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
