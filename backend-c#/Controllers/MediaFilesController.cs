using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.File;
using backend_c_.Service;
using Microsoft.Extensions.Logging;
using backend_c_.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace backend_c_.Controllers;

[ApiController]
[Route( "api/v1/[controller]" )]
[Authorize]
public class MediaFilesController : ControllerBase
{
  private readonly IFileService _fileService;
  private readonly ILogger<MediaFilesController> _logger;

  public MediaFilesController( IFileService fileService, ILogger<MediaFilesController> logger )
  {
    _fileService = fileService;
    _logger = logger;
  }

  // Upload File
  [HttpPost]
  public IActionResult Upload( [FromBody] UploadFileDto data )
  {
    LoggingHelper.LogRequest( _logger, "Received request to upload a file." );

    FileDto result = _fileService.Upload( data );

    LoggingHelper.LogSuccess( _logger, "File uploaded successfully", new { FileId = result.Id } );

    return CreatedAtAction(
      nameof( GetMyFiles ),
      new { userId = result.UserId },
      result
    );
  }

  // Update File
  [HttpPost( "{id}" )]
  public IActionResult Update( int id, [FromBody] UpdateFileDto data )
  {
    LoggingHelper.LogRequest( _logger, "Received request to update file", new { FileId = id } );

    FileDto result = _fileService.Update( id, data );
    if ( result == null )
    {
      LoggingHelper.LogFailure( _logger, "File not found", new { FileId = id } );
      return NotFound();
    }
    LoggingHelper.LogSuccess( _logger, "File updated successfully", new { FileId = id } );
    return Ok( result );
  }

  // Delete File
  [HttpDelete( "{id}" )]
  public IActionResult Remove( int id )
  {
    LoggingHelper.LogRequest( _logger, "Received request to remove file", new { FileId = id } );

    bool success = _fileService.Remove( id );
    if ( !success )
    {
      LoggingHelper.LogFailure( _logger, "Failed to remove file. File not found", new { FileId = id } );
      return NotFound();
    }
    LoggingHelper.LogSuccess( _logger, "File removed successfully", new { FileId = id } );
    return NoContent();
  }

  // Get All User Files
  [HttpGet( "my-files/{userId}" )]
  public IActionResult GetMyFiles( int userId )
  {
    LoggingHelper.LogRequest( _logger, "Received request to get files for user", new { UserId = userId } );

    IEnumerable<FileDto> result = _fileService.GetUserFiles( userId );

    LoggingHelper.LogSuccess( _logger, "Returning files for user", new { UserId = userId, FileCount = result.Count() } );

    return Ok( result );
  }

  // Get All Files Shared To Me
  [HttpGet( "shared-to-me/{userId}" )]
  public IActionResult GetFilesSharedToMe( int userId )
  {
    LoggingHelper.LogRequest( _logger, "Received request to get files shared to user", new { UserId = userId } );

    IEnumerable<FileDto> result = _fileService.GetFilesSharedToMe( userId );

    LoggingHelper.LogSuccess( _logger, "Returning files shared to user", new { UserId = userId, FileCount = result.Count() } );

    return Ok( result );
  }

  // Get All Files Shared By Me
  [HttpGet( "shared-by-me/{userId}" )]
  public IActionResult GetFilesSharedByMe( int userId )
  {
    LoggingHelper.LogRequest( _logger, "Received request to get files shared by user", new { UserId = userId } );

    IEnumerable<FileDto> result = _fileService.GetFilesSharedByMe( userId );

    LoggingHelper.LogSuccess( _logger, "Returning files shared by user", new { UserId = userId, FileCount = result.Count() } );

    return Ok( result );
  }
}
