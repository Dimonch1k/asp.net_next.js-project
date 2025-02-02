using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.SharedFile;
using backend_c_.Service;
using Microsoft.Extensions.Logging;
using backend_c_.Utilities;
using Microsoft.AspNetCore.Authorization;
using backend_c_.Enums;

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

  [HttpPost]
  public IActionResult Share( [FromBody] ShareFileDto data )
  {
    LoggingHelper.LogRequest( _logger, "share file", new { FileId = data.FileId } );

    ShareFileDto shareFileDto = _sharedFileService.Share( data );

    LoggingHelper.LogSuccess( _logger, "File shared successfully", new { FileId = shareFileDto.FileId } );

    return CreatedAtAction(
      nameof( Share ),
      new { id = shareFileDto.FileId },
      shareFileDto
    );
  }

  [HttpDelete( "{id}" )]
  public IActionResult Remove( int id )
  {
    LoggingHelper.LogRequest( _logger, "remove shared file", new { FileId = id } );

    ShareFileDto sharedFileDto = _sharedFileService.Remove( id );

    LoggingHelper.LogSuccess( _logger, "Shared file removed successfully", new { FileId = id } );

    return Ok( sharedFileDto );
  }
}
