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
  private readonly IUserService _userService;
  private readonly ILogger<SharedFilesController> _logger;

  public SharedFilesController( ISharedFileService sharedFileService, IFileService fileService, IUserService userService, ILogger<SharedFilesController> logger )
  {
    _sharedFileService = sharedFileService;
    _fileService = fileService;
    _userService = userService;
    _logger = logger;
  }

  [HttpGet]
  public IActionResult GetAllSharedFiles( )
  {
    _logger.LogInformation( "Received request to get all shared files" );

    IEnumerable<ShareFileDto> sharedFilesDto = _sharedFileService.GetAllSharedFiles();

    _logger.LogInformation( "Returning shared files" );

    return Ok( sharedFilesDto );
  }

  [HttpGet( "my-shares/{ownerId}" )]
  public IActionResult GetMySharedFiles( int ownerId )
  {
    _logger.LogInformation( $"Received request to get all shared files by user with ID: '{ownerId}'" );

    IEnumerable<ShareFileDto> sharedFilesDto = _sharedFileService.GetMySharedFiles( ownerId );

    _logger.LogInformation( "Returning shared files" );

    return Ok( sharedFilesDto );
  }

  [HttpPost]
  public async Task<IActionResult> ShareFile( [FromBody] ShareFileDto data )
  {
    _logger.LogInformation( "Received request to share file" );

    await _fileService.EnsureFileExists( data.FileId );
    await _userService.EnsureUserExists( data.OwnerId );
    await _userService.EnsureUserExists( data.SharedWithId );
    await _fileService.EnsureFileBelongsToSenderNotReceiver( data.FileId, data.OwnerId, data.SharedWithId );

    ShareFileDto shareFileDto = _sharedFileService.ShareFile( data );

    _logger.LogInformation( "File shared successfully" );

    return Ok( shareFileDto );
  }

  [HttpDelete( "{id}" )]
  public async Task<IActionResult> DeleteSharedFile( int id )
  {
    _logger.LogInformation( $"Received request to delete shared file with ID: {id}" );

    ShareFileDto sharedFileDto = await _sharedFileService.DeleteSharedFile( id );

    _logger.LogInformation( "Shared file deleted successfully" );

    return Ok( sharedFileDto );
  }
}
