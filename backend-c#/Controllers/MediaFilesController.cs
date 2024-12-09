using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.File;
using backend_c_.Service;
using Microsoft.Extensions.Logging;

namespace backend_c_.Controllers
{
  [ApiController]
  [Route( "api/v1/[controller]" )]
  public class MediaFilesController : ControllerBase
  {
    private readonly IFileService _fileService;
    private readonly ILogger<MediaFilesController> _logger;

    public MediaFilesController( IFileService fileService, ILogger<MediaFilesController> logger )
    {
      _fileService = fileService;
      _logger = logger;
    }

    [HttpPost( "upload" )]
    public IActionResult Upload( [FromBody] UploadFileDto data )
    {
      _logger.LogInformation( "Received request to upload a file." );

      FileDto result = _fileService.Upload( data );
      _logger.LogInformation( "File uploaded successfully with ID: {FileId}.", result.Id );
      return CreatedAtAction(
        nameof( GetMyFiles ),
        new { userId = result.UserId },
        result
      );
    }

    [HttpPost( "update/{id}" )]
    public IActionResult Update( int id, [FromBody] UpdateFileDto data )
    {
      _logger.LogInformation( "Received request to update file with ID: {FileId}.", id );

      FileDto result = _fileService.Update( id, data );
      if ( result == null )
      {
        _logger.LogWarning( "File with ID {FileId} not found.", id );
        return NotFound();
      }
      _logger.LogInformation( "File with ID {FileId} updated successfully.", id );
      return Ok( result );
    }

    [HttpDelete( "remove/{id}" )]
    public IActionResult Remove( int id )
    {
      _logger.LogInformation( "Received request to remove file with ID: {FileId}.", id );

      bool success = _fileService.Remove( id );
      if ( !success )
      {
        _logger.LogWarning( "Failed to remove file with ID {FileId}. File not found.", id );
        return NotFound();
      }
      _logger.LogInformation( "File with ID {FileId} removed successfully.", id );
      return NoContent();
    }

    [HttpGet( "my-files/{userId}" )]
    public IActionResult GetMyFiles( int userId )
    {
      _logger.LogInformation( "Received request to get files for user with ID: {UserId}.", userId );

      IEnumerable<FileDto> result = _fileService.GetUserFiles( userId );
      return Ok( result );
    }

    [HttpGet( "shared-to-me/{userId}" )]
    public IActionResult GetFilesSharedToMe( int userId )
    {
      _logger.LogInformation( "Received request to get files shared to user with ID: {UserId}.", userId );

      IEnumerable<FileDto> result = _fileService.GetFilesSharedToMe( userId );
      return Ok( result );
    }

    [HttpGet( "shared-by-me/{userId}" )]
    public IActionResult GetFilesSharedByMe( int userId )
    {
      _logger.LogInformation( "Received request to get files shared by user with ID: {UserId}.", userId );

      IEnumerable<FileDto> result = _fileService.GetFilesSharedByMe( userId );
      return Ok( result );
    }
  }
}
