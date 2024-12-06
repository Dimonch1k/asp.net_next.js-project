using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.SharedFile;
using backend_c_.Services.Interfaces;

namespace backend_c_.Controllers;

[ApiController]
[Route( "api/v1/[controller]" )]
public class SharedFilesController : ControllerBase
{
  private readonly ISharedFileService _sharedFileService;

  public SharedFilesController( ISharedFileService sharedFileService )
  {
    _sharedFileService = sharedFileService;
  }

  [HttpPost( "share" )]
  public IActionResult Share( [FromBody] ShareFileDto data )
  {
    ShareFileDto result = _sharedFileService.Share( data );
    return CreatedAtAction( nameof( Share ), new { id = result.FileId }, result );
  }

  [HttpDelete( "remove/{id}" )]
  public IActionResult Remove( int id )
  {
    bool success = _sharedFileService.Remove( id );
    return !success
      ? NotFound()
      : NoContent();
  }
}
