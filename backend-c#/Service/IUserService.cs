using backend_c_.DTO.User;
using backend_c_.Entity;

namespace backend_c_.Service;

public interface IUserService
{
  List<UserDto> FindAll( );
  UserDto FindOne( int id );
  UserDto Update( int id, UpdateUserDto data );
  UserDto Remove( int id );
  bool UserExists( string username, string email );
  User Register( RegisterDto registerDto );
}
