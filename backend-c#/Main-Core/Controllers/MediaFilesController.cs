using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.File;
using backend_c_.Service;
using Microsoft.Extensions.Logging;
using backend_c_.Utilities;
using Microsoft.AspNetCore.Authorization;
using backend_c_.DTO.SharedFile;
using backend_c_.Exceptions;

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

  [HttpPost]
  public async Task<IActionResult> Upload( IFormFile file, [FromForm] int userId )
  {
    LoggingHelper.LogRequest( _logger, "upload a file." );

    if ( file == null || file.Length == 0 )
    {
      LoggingHelper.LogFailure( _logger, "No file provided", new { UserId = userId } );

      throw new ServerException( "No file uploaded.", Enums.ExceptionStatusCode.NoFileProvided );
    }

    UploadFileDto uploadFileDto = new();
    uploadFileDto.UserId = userId;

    using ( MemoryStream memoryStream = new MemoryStream() )
    {
      await file.CopyToAsync( memoryStream );
      uploadFileDto.FileData = memoryStream.ToArray();
    }

    FileDto? uploadedFileDto = await _fileService.Upload( uploadFileDto, file );

    LoggingHelper.LogSuccess( _logger, "File uploaded successfully", new { FileId = uploadedFileDto.Id } );

    return Ok( uploadedFileDto );
  }

  [HttpPost( "{id}" )]
  public IActionResult Update( int id, [FromBody] UpdateFileDto data )
  {
    LoggingHelper.LogRequest( _logger, "update file", new { FileId = id } );

    FileDto fileDto = _fileService.Update( id, data );
    
    LoggingHelper.LogSuccess( _logger, "File updated successfully", new { FileId = id } );

    return Ok( fileDto );
  }

  [HttpDelete( "{id}" )]
  public IActionResult Remove( int id )
  {
    LoggingHelper.LogRequest( _logger, "remove file", new { FileId = id } );

    FileDto fileDto = _fileService.Remove( id );

    LoggingHelper.LogSuccess( _logger, "File removed successfully", new { FileId = id } );

    return Ok( fileDto );
  }

  [HttpGet( "my-files/{userId}" )]
  public IActionResult GetMyFiles( int userId )
  {
    LoggingHelper.LogRequest( _logger, "get files for user", new { UserId = userId } );

    IEnumerable<FileDto> userFilesDto = _fileService.GetUserFiles( userId );

    LoggingHelper.LogSuccess( _logger, "Returning files for user", new { UserId = userId, FileCount = userFilesDto.Count() } );

    return Ok( userFilesDto );
  }

  [HttpGet( "shared-to-me/{userId}" )]
  public IActionResult GetFilesSharedToMe( int userId )
  {
    LoggingHelper.LogRequest( _logger, "get files shared to user", new { UserId = userId } );

    IEnumerable<ShareFileDto> sharedToUserFilesDto = _fileService.GetFilesSharedToMe( userId );

    LoggingHelper.LogSuccess( _logger, "Returning files shared to user", new { UserId = userId, FileCount = sharedToUserFilesDto.Count() } );

    return Ok( sharedToUserFilesDto );
  }

  [HttpGet( "shared-by-me/{userId}" )]
  public IActionResult GetFilesSharedByMe( int userId )
  {
    LoggingHelper.LogRequest( _logger, "get files shared by user", new { UserId = userId } );

    IEnumerable<ShareFileDto> sharedByUserFilesDto = _fileService.GetFilesSharedByMe( userId );

    LoggingHelper.LogSuccess( _logger, "Returning files shared by user", new { UserId = userId, FileCount = sharedByUserFilesDto.Count() } );

    return Ok( sharedByUserFilesDto );
  }
}
