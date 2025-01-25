using backend_c_.DTO.User;

namespace backend_c_.Service;

public interface IAuthService
{
    string? Login(LoginDto loginDto);
    bool Authorize(AuthDto authDto);
}
