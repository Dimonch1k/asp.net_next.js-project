using backend_c_.DTO.User;
using backend_c_.Entity;

namespace backend_c_.Service;

public interface IUserService
{
  List<UserDto> GetAllUsers( );
  Task<UserDto> GetUserById( int id );
  Task<UserDto> UpdateUser( int id, UpdateUserDto data );
  Task<UserDto> DeleteUser( int id );
  Task<User> RegisterUser( RegisterDto registerDto );

  Task<User> GetUserIfExists( int? id );
  Task<User> GetUserByUsernameIfExists( string username );
  Task EnsureUserExists( int? id );
  Task EnsureUserIsUnique( string username, string email );
  string GetUserTimeZone( int? id );
}
