using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.Access;
using backend_c_.Services.Interfaces;

namespace backend_c_.Controllers;

[ApiController]
[Route( "api/v1/[controller]" )]
public class AccessController : ControllerBase
{
  private readonly IAccessLogService _accessLogService;

  public AccessController( IAccessLogService accessLogService )
  {
    _accessLogService = accessLogService;
  }

  [HttpGet( "findAll" )]
  public IActionResult FindAll( )
  {
    IEnumerable<AccessLogDto> result = _accessLogService.FindAll();
    return Ok( result );
  }

  [HttpGet( "findOne/{id}" )]
  public IActionResult FindOne( int id )
  {
    AccessLogDto result = _accessLogService.FindOne( id );
    return result == null
      ? NotFound()
      : Ok( result );
  }

  [HttpPost( "create" )]
  public IActionResult Create( [FromBody] CreateAccessLogDto createAccessLogDto )
  {
    AccessLogDto result = _accessLogService.Create( createAccessLogDto );
    return CreatedAtAction( nameof( FindOne ), new { id = result.Id }, result );
  }

  [HttpPatch( "update/{id}" )]
  public IActionResult Update( int id, [FromBody] UpdateAccessLogDto updateAccessLogDto )
  {
    AccessLogDto result = _accessLogService.Update( id, updateAccessLogDto );
    return result == null
      ? NotFound()
      : Ok( result );
  }

  [HttpDelete( "remove/{id}" )]
  public IActionResult Remove( int id )
  {
    bool success = _accessLogService.Remove( id );
    return !success
      ? NotFound()
      : NoContent();
  }

  [HttpGet( "file/{fileId}" )]
  public IActionResult FindAccessByFile( int fileId )
  {
    IEnumerable<AccessLogDto> result = _accessLogService.FindAccessByFile( fileId );
    return Ok( result );
  }

  [HttpGet( "user/{userId}" )]
  public IActionResult FindAccessByUser( int userId )
  {
    IEnumerable<AccessLogDto> result = _accessLogService.FindAccessByUser( userId );
    return Ok( result );
  }
}
