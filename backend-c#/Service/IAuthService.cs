using backend_c_.DTO.User;

namespace backend_c_.Service;

public interface IAuthService
{
  Task<string?> Login( LoginDto loginDto );
  bool Authorize( AuthDto authDto );
}
