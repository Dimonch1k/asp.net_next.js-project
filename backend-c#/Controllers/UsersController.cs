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
  public IActionResult FindAll( )
  {
    LoggingHelper.LogRequest( _logger, "find all users" );

    List<UserDto> usersDto = _userService.FindAll();

    LoggingHelper.LogSuccess( _logger, "Returning users", new { Count = usersDto.Count } );
    return Ok( usersDto );
  }

  [HttpGet( "{id}" )]
  public IActionResult FindOne( int id )
  {
    LoggingHelper.LogRequest( _logger, $"find user with ID: {id}" );

    UserDto userDto = _userService.FindOne( id );
    if ( userDto == null )
    {
      LoggingHelper.LogFailure( _logger, "User not found", new { Id = id } );
      return NotFound();
    }
    LoggingHelper.LogSuccess( _logger, "Returning user", new { Id = id } );
    return Ok( userDto );
  }

  [HttpPatch( "{id}" )]
  public IActionResult Update( int id, [FromBody] UpdateUserDto data )
  {
    LoggingHelper.LogRequest( _logger, $"update user with ID: {id}", data );

    UserDto updatedUserDto = _userService.Update( id, data );
    if ( updatedUserDto == null )
    {
      LoggingHelper.LogFailure( _logger, "User not found", new { Id = id } );
      return NotFound();
    }
    LoggingHelper.LogSuccess( _logger, "User updated successfully", new { Id = id } );
    return Ok( updatedUserDto );
  }

  [HttpDelete( "{id}" )]
  public IActionResult Remove( int id )
  {
    LoggingHelper.LogRequest( _logger, $"remove user with ID: {id}" );

    UserDto removedUserDto = _userService.Remove( id );
    if ( removedUserDto == null )
    {
      LoggingHelper.LogFailure( _logger, "Failed to remove user. User not found", new { Id = id } );
      return NotFound();
    }
    LoggingHelper.LogSuccess( _logger, "User removed successfully", new { Id = id } );
    return Ok(removedUserDto);
  }

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

  [AllowAnonymous]
  [HttpPost( "login" )]
  public IActionResult Login( [FromBody] LoginDto loginDto )
  {
    LoggingHelper.LogRequest( _logger, "login", loginDto );

    string? token = _authService.Login( loginDto );
    if ( string.IsNullOrEmpty( token ) )
    {
      LoggingHelper.LogFailure( _logger, "Invalid login attempt", loginDto );

      return Unauthorized( new { Message = "Invalid username or password" } );
    }

    LoggingHelper.LogSuccess( _logger, "User logged in successfully" );

    return Ok( new { Token = token } );
  }

  [AllowAnonymous]
  [HttpPost( "auth" )]
  public IActionResult Authorize( [FromBody] AuthDto authDto )
  {
    LoggingHelper.LogRequest( _logger, "authorize", authDto );

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
