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

  // Get All Users
  [HttpGet]
  public IActionResult FindAll( )
  {
    LoggingHelper.LogRequest( _logger, "find all users" );

    List<UserDto> users = _userService.FindAll();

    LoggingHelper.LogSuccess( _logger, "Returning users", new { Count = users.Count } );
    return Ok( users );
  }

  // Get One User
  [HttpGet( "{id}" )]
  public IActionResult FindOne( int id )
  {
    LoggingHelper.LogRequest( _logger, $"find user with ID: {id}" );

    UserDto user = _userService.FindOne( id );
    if ( user == null )
    {
      LoggingHelper.LogFailure( _logger, "User not found", new { Id = id } );
      return NotFound();
    }
    LoggingHelper.LogSuccess( _logger, "Returning user", new { Id = id } );
    return Ok( user );
  }

  // Update User
  [HttpPatch( "{id}" )]
  public IActionResult Update( int id, [FromBody] UpdateUserDto data )
  {
    LoggingHelper.LogRequest( _logger, $"update user with ID: {id}", data );

    UserDto updatedUser = _userService.Update( id, data );
    if ( updatedUser == null )
    {
      LoggingHelper.LogFailure( _logger, "User not found", new { Id = id } );
      return NotFound();
    }
    LoggingHelper.LogSuccess( _logger, "User updated successfully", new { Id = id } );
    return Ok( updatedUser );
  }

  // Delete User
  [HttpDelete( "{id}" )]
  public IActionResult Remove( int id )
  {
    LoggingHelper.LogRequest( _logger, $"remove user with ID: {id}" );

    bool success = _userService.Remove( id );
    if ( !success )
    {
      LoggingHelper.LogFailure( _logger, "Failed to remove user. User not found", new { Id = id } );
      return NotFound();
    }
    LoggingHelper.LogSuccess( _logger, "User removed successfully", new { Id = id } );
    return NoContent();
  }

  // Registration
  [AllowAnonymous]
  [HttpPost( "register" )]
  public IActionResult Register( [FromBody] RegisterDto registerDto )
  {
    LoggingHelper.LogRequest( _logger, "register a new user", registerDto );

    if ( _userService.UserExists( registerDto.Username, registerDto.Email ) )
    {
      LoggingHelper.LogFailure( _logger, "Username or email already exists", registerDto );

      return Conflict( new { Message = "Username or email already exists" } );
    }

    User newUser = _userService.Register( registerDto );

    LoggingHelper.LogSuccess( _logger, "User created successfully", new { Id = newUser.Id } );

    return CreatedAtAction(
      nameof( Login ),
      new { id = newUser.Id },
      new { Message = "User created successfully" }
    );
  }

  // Login
  [AllowAnonymous]
  [HttpPost( "login" )]
  public IActionResult Login( [FromBody] LoginDto loginDto )
  {
    LoggingHelper.LogRequest( _logger, "login request", loginDto );

    string token = _authService.Login( loginDto );
    if ( string.IsNullOrEmpty( token ) )
    {
      LoggingHelper.LogFailure( _logger, "Invalid login attempt", loginDto );

      return Unauthorized( new { Message = "Invalid username or password" } );
    }

    LoggingHelper.LogSuccess( _logger, "User logged in successfully" );

    return Ok( new { Token = token } );
  }

  // Authorization
  [AllowAnonymous]
  [HttpPost( "auth" )]
  public IActionResult Authorize( [FromBody] AuthDto authDto )
  {
    LoggingHelper.LogRequest( _logger, "authorization request", authDto );

    bool isAuthorized = _authService.Authorize( authDto );
    if ( !isAuthorized )
    {
      LoggingHelper.LogFailure( _logger, "Authorization failed" );
      return Forbid();
    }
    LoggingHelper.LogSuccess( _logger, "User authorized successfully" );

    return Ok( new { Message = "Authorization successful" } );
  }
}
