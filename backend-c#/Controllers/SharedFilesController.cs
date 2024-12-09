using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.SharedFile;
using backend_c_.Service;
using Microsoft.Extensions.Logging;

namespace backend_c_.Controllers
{
  [ApiController]
  [Route( "api/v1/[controller]" )]
  public class SharedFilesController : ControllerBase
  {
    private readonly ISharedFileService _sharedFileService;
    private readonly ILogger<SharedFilesController> _logger;

    public SharedFilesController( ISharedFileService sharedFileService, ILogger<SharedFilesController> logger )
    {
      _sharedFileService = sharedFileService;
      _logger = logger;
    }

    [HttpPost( "share" )]
    public IActionResult Share( [FromBody] ShareFileDto data )
    {
      _logger.LogInformation( "Received request to share file with ID: {FileId}.", data.FileId );

      ShareFileDto result = _sharedFileService.Share( data );

      _logger.LogInformation( "File with ID: {FileId} shared successfully.", result.FileId );

      return CreatedAtAction( nameof( Share ), new { id = result.FileId }, result );
    }

    [HttpDelete( "remove/{id}" )]
    public IActionResult Remove( int id )
    {
      _logger.LogInformation( "Received request to remove shared file with ID: {FileId}.", id );

      bool success = _sharedFileService.Remove( id );
      if ( !success )
      {
        _logger.LogWarning( "Failed to remove shared file with ID {FileId}. File not found.", id );
        return NotFound();
      }
      _logger.LogInformation( "Shared file with ID {FileId} removed successfully.", id );
      return NoContent();
    }
  }
}
