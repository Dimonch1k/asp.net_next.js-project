using backend_c_.DTO.User;
using backend_c_.DTO.Version;
using backend_c_.Entity;
using backend_c_.Exceptions.User;
using backend_c_.Utils;

namespace backend_c_.Service.Impl;

public class UserService : IUserService
{
  private AppDbContext _dbContext;

  public UserService( AppDbContext dbContext )
  {
    _dbContext = dbContext;
  }

  public List<UserDto> FindAll( )
  {
    return _dbContext.Users
      .Select( user => UserToDto( user ) )
      .ToList();
  }

  public UserDto FindOne( int id )
  {
    User? user = _dbContext.Users.FirstOrDefault( u => u.Id == id );

    if ( user == null )
    {
      throw new UserNotFoundException( id );
    }

    return UserToDto( user );
  }

  public User Register( RegisterDto registerDto )
  {
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

  public UserDto Remove( int id )
  {
    User? user = _dbContext.Users.FirstOrDefault( u => u.Id == id );

    if ( user == null )
    {
      throw new UserNotFoundException( id );
    }

    _dbContext.Users.Remove( user );
    _dbContext.SaveChanges();

    return UserToDto( user );
  }

  public UserDto Update( int id, UpdateUserDto data )
  {
    User? user = _dbContext.Users.FirstOrDefault( u => u.Id == id );

    if ( user == null )
    {
      throw new Exception( $"User with ID {id} not found" );
    }

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


  public bool UserExists( string username, string email )
  {
    return _dbContext.Users.Any( user =>
      user.Username == username
      && user.Email == email
    );
  }

  private static UserDto UserToDto( User user )
  {
    return new UserDto
    {
      Id = user.Id,
      Username = user.Username,
      Email = user.Email,
    };
  }
}
