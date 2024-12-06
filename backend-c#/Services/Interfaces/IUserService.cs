using backend_c_.DTO.User;
using backend_c_.Entity;

namespace backend_c_.Services.Interfaces;

public interface IUserService
{
  List<UserDto> FindAll( );
  UserDto FindOne( int id );
  UserDto Update( int id, UpdateUserDto data );
  bool Remove( int id );
  bool UserExists( string username, string email );
  User Register( RegisterDto registerDto );
}
