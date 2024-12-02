using backend_c_.DTO.User;

namespace backend_c_.Services.Interfaces;

public interface IAuthService
{
  string Login( LoginDto loginDto );
  bool Authorize( AuthDto authDto );
}
