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
  public async Task<IActionResult> UploadFile( IFormFile file, [FromForm] int userId )
  {
    _logger.LogInformation( "Received request to upload a file." );

    if ( file == null || file.Length == 0 )
    {
      _logger.LogError( "No file provided" );

      throw new ServerException( "No file provided.", Enums.ExceptionStatusCode.NoFileProvided );
    }

    UploadFileDto uploadFileDto = new();
    uploadFileDto.UserId = userId;

    using ( MemoryStream memoryStream = new MemoryStream() )
    {
      await file.CopyToAsync( memoryStream );
      uploadFileDto.FileData = memoryStream.ToArray();
    }

    FileDto? uploadedFileDto = await _fileService.UploadFile( uploadFileDto, file );

    _logger.LogInformation( "File uploaded successfully" );

    return Ok( uploadedFileDto );
  }

  [HttpPost( "{id}" )]
  public async Task<IActionResult> UpdateFile( int id, [FromBody] UpdateFileDto data )
  {
    _logger.LogInformation( $"Received request to update file with ID: {id}" );

    FileDto fileDto = await _fileService.UpdateFile( id, data );

    _logger.LogInformation( "File updated successfully" );

    return Ok( fileDto );
  }

  [HttpDelete( "{id}" )]
  public async Task<IActionResult> DeleteFile( int id )
  {
    _logger.LogInformation( $"Received request to delete file with ID: {id}" );

    FileDto fileDto = await _fileService.DeleteFile( id );

    _logger.LogInformation( "File deleted successfully" );

    return Ok( fileDto );
  }

  [HttpGet( "my-files/{userId}" )]
  public async Task<IActionResult> GetMyFiles( int userId )
  {
    _logger.LogInformation( $"Received request to get files for user with ID: {userId}" );

    IEnumerable<FileDto> userFilesDto = await _fileService.GetUserFiles( userId );

    _logger.LogInformation( "Returning files for user" );

    return Ok( userFilesDto );
  }

  [HttpGet( "shared-to-me/{userId}" )]
  public async Task<IActionResult> GetFilesSharedToMe( int userId )
  {
    _logger.LogInformation( $"Received request to get files shared to user with ID: {userId}" );

    IEnumerable<ShareFileDto> sharedToUserFilesDto = await _fileService.GetFilesSharedToMe( userId );

    _logger.LogInformation( "Returning files shared to user" );

    return Ok( sharedToUserFilesDto );
  }

  [HttpGet( "shared-by-me/{userId}" )]
  public async Task<IActionResult> GetFilesSharedByMe( int userId )
  {
    _logger.LogInformation( $"Received request to get files shared by user with ID: {userId}" );

    IEnumerable<ShareFileDto> sharedByUserFilesDto = await _fileService.GetFilesSharedByMe( userId );

    _logger.LogInformation( "Returning files shared by user" );

    return Ok( sharedByUserFilesDto );
  }
}
