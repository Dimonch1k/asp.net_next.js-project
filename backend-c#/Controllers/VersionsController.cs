using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.Version;
using backend_c_.Services;

namespace backend_c_.Controllers;

[ApiController]
[Route( "api/v1/[controller]" )]
public class VersionsController : ControllerBase
{
  private readonly IVersionService _versionService;

  public VersionsController( IVersionService versionService )
  {
    _versionService = versionService;
  }

  [HttpPost( "create" )]
  public IActionResult Create( [FromBody] CreateFileVersionDto data )
  {
    FileVersionDto result = _versionService.Create( data );
    return CreatedAtAction( nameof( FindOne ), new { id = result.Id }, result );
  }

  [HttpGet( "findAll" )]
  public IActionResult FindAll( )
  {
    IEnumerable<FileVersionDto> result = _versionService.FindAll();
    return Ok( result );
  }

  [HttpGet( "findOne/{id}" )]
  public IActionResult FindOne( int id )
  {
    FileVersionDto result = _versionService.FindOne( id );
    return result == null
      ? NotFound()
      : Ok( result );
  }

  [HttpPatch( "update/{id}" )]
  public IActionResult Update( int id, [FromBody] UpdateFileVersionDto data )
  {
    FileVersionDto result = _versionService.Update( id, data );
    return result == null
      ? NotFound()
      : Ok( result );
  }

  [HttpDelete( "remove/{id}" )]
  public IActionResult Remove( int id )
  {
    bool success = _versionService.Remove( id );
    return !success
      ? NotFound()
      : NoContent();
  }

  [HttpGet( "findByFileId/{fileId}" )]
  public IActionResult FindByFileId( int fileId )
  {
    IEnumerable<FileVersionDto> result = _versionService.FindByFileId( fileId );
    return Ok( result );
  }
}
