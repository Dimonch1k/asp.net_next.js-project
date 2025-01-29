using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.Version;
using backend_c_.Service;
using Microsoft.Extensions.Logging;
using backend_c_.Utilities;
using System;

namespace backend_c_.Controllers;

[ApiController]
[Route( "api/v1/[controller]" )]
public class VersionsController : ControllerBase
{
  private readonly IVersionService _versionService;
  private readonly ILogger<VersionsController> _logger;

  public VersionsController( IVersionService versionService, ILogger<VersionsController> logger )
  {
    _versionService = versionService;
    _logger = logger;
  }

  [HttpPost]
  public IActionResult Create( [FromBody] CreateFileVersionDto data )
  {
    LoggingHelper.LogRequest( _logger, "create file version", new { FileId = data.FileId } );

    FileVersionDto fileVersionDto = _versionService.Create( data );

    LoggingHelper.LogSuccess( _logger, "File version created successfully", new { VersionId = fileVersionDto.Id } );

    return CreatedAtAction(
      nameof( FindOne ),
      new { id = fileVersionDto.Id },
      fileVersionDto
    );
  }

  [HttpPost( "{versionId}/restore" )]
  public IActionResult RestoreVersion( int versionId )
  {
    LoggingHelper.LogRequest( _logger, "restore file version", new {VersionId = versionId} );

    FileVersionDto restoredVersionDto = _versionService.RestoreVersion( versionId );

    LoggingHelper.LogSuccess( _logger, "File version restored successfully", new { VersionId = versionId} );

    return Ok( restoredVersionDto );
  }

  [HttpGet]
  public IActionResult FindAll( )
  {
    LoggingHelper.LogRequest( _logger, "find all file versions" );

    IEnumerable<FileVersionDto> fileVersionsDto = _versionService.FindAll();

    LoggingHelper.LogSuccess( _logger, "Returning file versions", new { FileVersionCount = fileVersionsDto.Count() } );

    return Ok( fileVersionsDto );
  }

  [HttpGet( "{id}" )]
  public IActionResult FindOne( int id )
  {
    LoggingHelper.LogRequest( _logger, "find file version by ID", new { VersionId = id } );

    FileVersionDto fileVersionDto = _versionService.FindOne( id );

    LoggingHelper.LogSuccess( _logger, "Returning file version", new { VersionId = id } );

    return Ok( fileVersionDto );
  }

  [HttpPatch( "{id}" )]
  public IActionResult Update( int id, [FromBody] UpdateFileVersionDto data )
  {
    LoggingHelper.LogRequest( _logger, "update file version", new { VersionId = id } );

    FileVersionDto fileVersionDto = _versionService.Update( id, data );

    LoggingHelper.LogSuccess( _logger, "File version updated successfully", new { VersionId = id } );
    
    return Ok( fileVersionDto );
  }

  [HttpDelete( "{id}" )]
  public IActionResult Remove( int id )
  {
    LoggingHelper.LogRequest( _logger, "remove file version", new { VersionId = id } );

    FileVersionDto removedFileVersionDto = _versionService.Remove( id );

    LoggingHelper.LogSuccess( _logger, "File version removed successfully", new { VersionId = id } );

    return Ok( removedFileVersionDto );
  }

  [HttpGet( "findByFileId/{fileId}" )]
  public IActionResult FindByFileId( int fileId )
  {
    LoggingHelper.LogRequest( _logger, "find file versions by file ID", new { FileId = fileId } );

    return Ok( _versionService.FindByFileId( fileId ) );
  }
}
