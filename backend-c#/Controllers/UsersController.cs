using backend_c_.DTO.User;
using backend_c_.Entity;
using backend_c_.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend_c_.Controllers;

[ApiController]
[Route( "api/v1/[controller]" )]
public class UsersController : ControllerBase
{
  private readonly IUserService _userService;
  private readonly IAuthService _authService;

  public UsersController( IUserService userService, IAuthService authService )
  {
    _userService = userService;
    _authService = authService;
  }

  [HttpGet( "findAll" )]
  public IActionResult FindAll( )
  {
    List<UserDto> users = _userService.FindAll();
    return Ok( users );
  }

  [HttpGet( "findOne/{id}" )]
  public IActionResult FindOne( int id )
  {
    UserDto user = _userService.FindOne( id );
    return user == null
      ? NotFound()
      : Ok( user );
  }

  [HttpPatch( "update/{id}" )]
  public IActionResult Update( int id, [FromBody] UpdateUserDto data )
  {
    UserDto updatedUser = _userService.Update( id, data );
    return updatedUser == null
      ? NotFound()
      : Ok( updatedUser );
  }

  [HttpDelete( "remove/{id}" )]
  public IActionResult Remove( int id )
  {
    bool success = _userService.Remove( id );
    return !success
      ? NotFound()
      : NoContent();
  }

  [HttpPost( "register" )]
  public IActionResult Register( [FromBody] RegisterDto registerDto )
  {
    if ( _userService.UserExists( registerDto.Username, registerDto.Email ) )
    {
      return Conflict( new { Message = "Username or email already exists" } );
    }

    User newUser = _userService.Register( registerDto );

    return CreatedAtAction(
      nameof( Login ),
      new { id = newUser.Id },
      new { Message = "User created successfully" } );
  }

  [HttpPost( "login" )]
  public IActionResult Login( [FromBody] LoginDto loginDto )
  {
    string token = _authService.Login( loginDto );
    return string.IsNullOrEmpty( token )
      ? Unauthorized( new { Message = "Invalid username or password" } )
      : Ok( new { Token = token } );
  }

  [HttpPost( "auth" )]
  public IActionResult Authorize( [FromBody] AuthDto authDto )
  {
    bool isAuthorized = _authService.Authorize( authDto );
    return !isAuthorized
      ? Forbid()
      : Ok( new { Message = "Authorization successful" } );
  }
}
