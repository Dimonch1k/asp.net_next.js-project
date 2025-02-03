using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.SharedFile;
using backend_c_.Service;
using Microsoft.Extensions.Logging;
using backend_c_.Utilities;
using Microsoft.AspNetCore.Authorization;
using backend_c_.Enums;
using backend_c_.Entity;

namespace backend_c_.Controllers;

[ApiController]
[Route( "api/v1/[controller]" )]
[Authorize]
public class SharedFilesController : ControllerBase
{
  private readonly ISharedFileService _sharedFileService;
  private readonly IFileService _fileService;
  private readonly ILogger<SharedFilesController> _logger;

  public SharedFilesController( ISharedFileService sharedFileService, IFileService fileService, ILogger<SharedFilesController> logger )
  {
    _sharedFileService = sharedFileService;
    _fileService = fileService;
    _logger = logger;
  }

  [HttpPost]
  public IActionResult ShareFile( [FromBody] ShareFileDto data )
  {
    _logger.LogInformation( "Received request to share file" );

    _fileService.EnsureFileExists( data.FileId );

    ShareFileDto shareFileDto = _sharedFileService.ShareFile( data );

    _logger.LogInformation( "File shared successfully" );

    return Ok( shareFileDto );
  }

  [HttpDelete( "{id}" )]
  public IActionResult DeleteSharedFile( int id )
  {
    _logger.LogInformation( $"Received request to delete shared file with ID: {id}" );

    ShareFileDto sharedFileDto = _sharedFileService.DeleteSharedFile( id );

    _logger.LogInformation( "Shared file deleted successfully" );

    return Ok( sharedFileDto );
  }
}
