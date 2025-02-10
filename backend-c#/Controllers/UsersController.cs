using backend_c_.DTO.User;
using backend_c_.Entity;
using backend_c_.Service;
using backend_c_.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace backend_c_.Controllers;

[ApiController]
[Route( "api/v1/[controller]" )]
[Authorize]
public class UsersController : ControllerBase
{
  private readonly IUserService _userService;
  private readonly IAuthService _authService;
  private readonly ILogger<UsersController> _logger;

  public UsersController( IUserService userService, IAuthService authService, ILogger<UsersController> logger )
  {
    _userService = userService;
    _authService = authService;
    _logger = logger;
  }

  [HttpGet]
  public IActionResult GetAllUsers( )
  {
    _logger.LogInformation( "Received request to get all users" );

    List<UserDto> usersDto = _userService.GetAllUsers();

    _logger.LogInformation( "Returning users" );

    return Ok( usersDto );
  }

  [HttpGet( "{id}" )]
  public async Task<IActionResult> GetUserById( int id )
  {
    _logger.LogInformation( $"Received request to get user with ID: {id}" );

    UserDto userDto = await _userService.GetUserById( id );

    _logger.LogInformation( "Returning user" );

    return Ok( userDto );
  }

  [HttpPatch( "{id}" )]
  public async Task<IActionResult> UpdateUser( int id, [FromBody] UpdateUserDto data )
  {
    _logger.LogInformation( $"Received request to update user with ID: {id}" );

    UserDto updatedUserDto = await _userService.UpdateUser( id, data );

    _logger.LogInformation( "User updated successfully" );

    return Ok( updatedUserDto );
  }

  [HttpDelete( "{id}" )]
  public async Task<IActionResult> DeleteUser( int id )
  {
    _logger.LogInformation( $"Received request to delete user with ID: {id}" );

    UserDto deletedUserDto = await _userService.DeleteUser( id );

    _logger.LogInformation( "User deleted successfully" );

    return Ok( deletedUserDto );
  }

  [AllowAnonymous]
  [HttpPost( "register" )]
  public async Task<IActionResult> RegisterUser( [FromBody] RegisterDto registerDto )
  {
    _logger.LogInformation( "Received request to register a new user" );

    User newUser = await _userService.RegisterUser( registerDto );

    _logger.LogInformation( "Register successful" );

    return Ok( "Register successful" );
  }

  [AllowAnonymous]
  [HttpPost( "login" )]
  public async Task<IActionResult> Login( [FromBody] LoginDto loginDto )
  {
    _logger.LogInformation( "Received request to login" );

    string? token = await _authService.Login( loginDto );

    _logger.LogInformation( "User logged in successfully" );

    return Ok( new { Token = token } );
  }

  [AllowAnonymous]
  [HttpPost( "auth" )]
  public IActionResult Authorize( [FromBody] AuthDto authDto )
  {
    _logger.LogInformation( "Received request to authorize" );

    bool isAuthorized = _authService.Authorize( authDto );

    _logger.LogInformation( "User authorized successfully" );

    return Ok( "Authorization successful" );
  }
}
