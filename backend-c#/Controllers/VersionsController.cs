using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.Version;
using backend_c_.Service;
using Microsoft.Extensions.Logging;
using backend_c_.Utilities;
using System;
using backend_c_.Entity;

namespace backend_c_.Controllers;

[ApiController]
[Route( "api/v1/[controller]" )]
public class VersionsController : ControllerBase
{
  private readonly IVersionService _versionService;
  private readonly IFileService _fileService;
  private readonly ILogger<VersionsController> _logger;

  public VersionsController( IVersionService versionService, IFileService fileService, ILogger<VersionsController> logger )
  {
    _versionService = versionService;
    _fileService = fileService;
    _logger = logger;
  }

  [HttpGet]
  public IActionResult GetAllVersions( )
  {
    _logger.LogInformation( "Received request to find all file versions" );

    IEnumerable<FileVersionDto> fileVersionsDto = _versionService.GetAllVersions();

    _logger.LogInformation( "Returning file versions" );

    return Ok( fileVersionsDto );
  }

  [HttpGet( "getByFileId/{fileId}" )]
  public IActionResult GetVersionsByFileId( int fileId )
  {
    _logger.LogInformation( $"Received request to find file versions with file ID: {fileId}" );

    _fileService.EnsureFileExists( fileId );

    IEnumerable<FileVersionDto> fileVersionsDto = _versionService.GetVersionsByFileId( fileId );

    _logger.LogInformation( "Returning file versions" );

    return Ok( fileVersionsDto );
  }

  [HttpGet( "{id}" )]
  public IActionResult GetVersionById( int id )
  {
    _logger.LogInformation( $"Received request to find file version by ID: {id}" );

    FileVersionDto fileVersionDto = _versionService.GetVersionById( id );

    _logger.LogInformation( "Returning file version" );

    return Ok( fileVersionDto );
  }

  [HttpPost]
  public IActionResult CreateVersion( [FromBody] CreateFileVersionDto data )
  {
    _logger.LogInformation( "Received request to create file version" );

    MediaFile? file = _fileService.GetFileIfExists( data.FileId );

    FileVersionDto fileVersionDto = _versionService.CreateVersion( data, file );

    _logger.LogInformation( "File version created successfully" );

    return Ok( fileVersionDto );
  }

  [HttpPost( "{versionId}/restore" )]
  public IActionResult RestoreFileVersion( int versionId )
  {
    _logger.LogInformation( $"Received request to restore file version with ID: {versionId}" );

    FileVersion? fileVersion = _versionService.GetFileVersionIfExists( versionId );

    MediaFile? file = _fileService.GetFileIfExists( fileVersion.FileId );

    _fileService.EnsureFileIsNotNull( file );

    FileVersionDto restoredVersionDto = _versionService.RestoreFileVersion( versionId, file, fileVersion );

    _logger.LogInformation( "File version restored successfully" );

    return Ok( restoredVersionDto );
  }

  [HttpPatch( "{id}" )]
  public IActionResult UpdateVersion( int id, [FromBody] UpdateFileVersionDto data )
  {
    _logger.LogInformation( $"Received request to update file version with ID: {id}" );

    FileVersionDto fileVersionDto = _versionService.UpdateVersion( id, data );

    _logger.LogInformation( "File version updated successfully" );

    return Ok( fileVersionDto );
  }

  [HttpDelete( "{id}" )]
  public IActionResult DeleteVersion( int id )
  {
    _logger.LogInformation( $"Received request to delete file version with ID: {id}" );

    FileVersionDto deletedFileVersionDto = _versionService.DeleteVersion( id );

    _logger.LogInformation( "File version deleted successfully" );

    return Ok( deletedFileVersionDto );
  }

}
