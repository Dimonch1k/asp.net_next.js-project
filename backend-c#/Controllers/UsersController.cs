using backend_c_.DTO.User;
using backend_c_.Entity;
using backend_c_.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace backend_c_.Controllers
{
  [ApiController]
  [Route( "api/v1/[controller]" )]
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

    [HttpGet( "findAll" )]
    public IActionResult FindAll( )
    {
      _logger.LogInformation( "Received request to find all users." );

      List<UserDto> users = _userService.FindAll();
      _logger.LogInformation( "Returning {UserCount} users.", users.Count );
      return Ok( users );
    }

    [HttpGet( "findOne/{id}" )]
    public IActionResult FindOne( int id )
    {
      _logger.LogInformation( "Received request to find user with ID: {UserId}", id );

      UserDto user = _userService.FindOne( id );
      if ( user == null )
      {
        _logger.LogWarning( "User with ID {UserId} not found.", id );
        return NotFound();
      }
      _logger.LogInformation( "Returning user with ID: {UserId}.", id );
      return Ok( user );
    }

    [HttpPatch( "update/{id}" )]
    public IActionResult Update( int id, [FromBody] UpdateUserDto data )
    {
      _logger.LogInformation( "Received request to update user with ID: {UserId}.", id );

      UserDto updatedUser = _userService.Update( id, data );
      if ( updatedUser == null )
      {
        _logger.LogWarning( "User with ID {UserId} not found.", id );
        return NotFound();
      }
      _logger.LogInformation( "User with ID {UserId} updated successfully.", id );
      return Ok( updatedUser );
    }

    [HttpDelete( "remove/{id}" )]
    public IActionResult Remove( int id )
    {
      _logger.LogInformation( "Received request to remove user with ID: {UserId}.", id );

      bool success = _userService.Remove( id );
      if ( !success )
      {
        _logger.LogWarning( "Failed to remove user with ID {UserId}. User not found.", id );
        return NotFound();
      }
      _logger.LogInformation( "User with ID {UserId} removed successfully.", id );
      return NoContent();
    }

    [HttpPost( "register" )]
    public IActionResult Register( [FromBody] RegisterDto registerDto )
    {
      _logger.LogInformation( "Received request to register a new user with username: {Username}", registerDto.Username );

      if ( _userService.UserExists( registerDto.Username, registerDto.Email ) )
      {
        _logger.LogWarning( "Username or email already exists: {Username}, {Email}", registerDto.Username, registerDto.Email );

        return Conflict( new { Message = "Username or email already exists" } );
      }

      User newUser = _userService.Register( registerDto );
      _logger.LogInformation( "User created successfully with ID: {UserId}", newUser.Id );

      return CreatedAtAction(
        nameof( Login ),
        new { id = newUser.Id },
        new { Message = "User created successfully" }
      );
    }

    [HttpPost( "login" )]
    public IActionResult Login( [FromBody] LoginDto loginDto )
    {
      _logger.LogInformation( "Received login request for username: {Username}", loginDto.Username );

      string token = _authService.Login( loginDto );
      if ( string.IsNullOrEmpty( token ) )
      {
        _logger.LogWarning( "Invalid login attempt for username: {Username}", loginDto.Username );
        return Unauthorized( new { Message = "Invalid username or password" } );
      }
      _logger.LogInformation( "User logged in successfully, token generated." );
      return Ok( new { Token = token } );
    }

    [HttpPost( "auth" )]
    public IActionResult Authorize( [FromBody] AuthDto authDto )
    {
      _logger.LogInformation( "Received authorization request for user" );

      bool isAuthorized = _authService.Authorize( authDto );
      if ( !isAuthorized )
      {
        _logger.LogWarning( "User failed authorization." );
        return Forbid();
      }
      _logger.LogInformation( "User authorized successfully." );
      return Ok( new { Message = "Authorization successful" } );
    }
  }
}
