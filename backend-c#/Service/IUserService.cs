using backend_c_.DTO.User;
using backend_c_.Entity;

namespace backend_c_.Service;

public interface IUserService
{
  List<UserDto> GetAllUsers( );
  UserDto GetUserById( int id );
  UserDto UpdateUser( int id, UpdateUserDto data );
  UserDto DeleteUser( int id );
  User RegisterUser( RegisterDto registerDto );

  User GetUserIfExists( int id );
  void EnsureUserExists( int id );
  void EnsureUserIsNotNull( User? user );
  void EnsureUserIsUnique( string username, string email );
}
