using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.File;
using backend_c_.Services.Interfaces;

namespace backend_c_.Controllers;

[ApiController]
[Route( "api/v1/[controller]" )]
public class FilesController : ControllerBase
{
  private readonly IFileService _fileService;

  public FilesController( IFileService fileService )
  {
    _fileService = fileService;
  }

  [HttpPost( "upload" )]
  public IActionResult Upload( [FromBody] UploadFileDto data )
  {
    FileDto result = _fileService.Upload( data );
    return CreatedAtAction( nameof( GetMyFiles ), new { userId = result.UserId }, result );
  }

  [HttpPost( "update/{id}" )]
  public IActionResult Update( int id, [FromBody] UpdateFileDto data )
  {
    FileDto result = _fileService.Update( id, data );
    return result == null
      ? NotFound()
      : Ok( result );
  }

  [HttpDelete( "remove/{id}" )]
  public IActionResult Remove( int id )
  {
    bool success = _fileService.Remove( id );
    return !success
      ? NotFound()
      : NoContent();
  }

  [HttpGet( "my-files/{userId}" )]
  public IActionResult GetMyFiles( int userId )
  {
    IEnumerable<FileDto> result = _fileService.GetUserFiles( userId );
    return Ok( result );
  }

  [HttpGet( "shared-to-me/{userId}" )]
  public IActionResult GetFilesSharedToMe( int userId )
  {
    IEnumerable<FileDto> result = _fileService.GetFilesSharedToMe( userId );
    return Ok( result );
  }

  [HttpGet( "shared-by-me/{userId}" )]
  public IActionResult GetFilesSharedByMe( int userId )
  {
    IEnumerable<FileDto> result = _fileService.GetFilesSharedByMe( userId );
    return Ok( result );
  }
}
