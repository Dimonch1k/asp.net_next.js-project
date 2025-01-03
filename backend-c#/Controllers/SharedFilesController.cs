using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.SharedFile;
using backend_c_.Service;
using Microsoft.Extensions.Logging;
using backend_c_.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace backend_c_.Controllers;

[ApiController]
[Route( "api/v1/[controller]" )]
[Authorize]
public class SharedFilesController : ControllerBase
{
  private readonly ISharedFileService _sharedFileService;
  private readonly ILogger<SharedFilesController> _logger;

  public SharedFilesController( ISharedFileService sharedFileService, ILogger<SharedFilesController> logger )
  {
    _sharedFileService = sharedFileService;
    _logger = logger;
  }

  // Share File
  [HttpPost]
  public IActionResult Share( [FromBody] ShareFileDto data )
  {
    LoggingHelper.LogRequest( _logger, "Share file", new { FileId = data.FileId } );

    ShareFileDto result = _sharedFileService.Share( data );

    LoggingHelper.LogSuccess( _logger, "File shared successfully", new { FileId = result.FileId } );
    return CreatedAtAction(
      nameof( Share ),
      new { id = result.FileId },
      result
    );
  }

  // Delete Shared File
  [HttpDelete( "{id}" )]
  public IActionResult Remove( int id )
  {
    LoggingHelper.LogRequest( _logger, "Remove shared file", new { FileId = id } );

    bool success = _sharedFileService.Remove( id );
    if ( !success )
    {
      LoggingHelper.LogFailure( _logger, "Failed to remove shared file. File not found", new { FileId = id } );
      return NotFound();
    }
    LoggingHelper.LogSuccess( _logger, "Shared file removed successfully", new { FileId = id } );
    return NoContent();
  }
}
