using Microsoft.AspNetCore.Mvc;
using backend_c_.Dtos;
using backend_c_.Services;

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
    return CreatedAtAction( nameof( Share ), new { id = result.Id }, result );
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
