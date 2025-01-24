using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.Version;
using backend_c_.Service;
using Microsoft.Extensions.Logging;
using backend_c_.Utilities;

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
    LoggingHelper.LogRequest( _logger, "Create file version", new { FileId = data.FileId } );

    FileVersionDto result = _versionService.Create( data );

    LoggingHelper.LogSuccess( _logger, "File version created successfully", new { VersionId = result.Id } );
    return CreatedAtAction(
      nameof( FindOne ),
      new { id = result.Id },
      result
    );
  }

  [HttpGet]
  public IActionResult FindAll( )
  {
    LoggingHelper.LogRequest( _logger, "Find all file versions" );

    IEnumerable<FileVersionDto> result = _versionService.FindAll();

    LoggingHelper.LogSuccess( _logger, "Returning file versions", new { FileVersionCount = result.Count() } );
    return Ok( result );
  }

  [HttpGet( "{id}" )]
  public IActionResult FindOne( int id )
  {
    LoggingHelper.LogRequest( _logger, "Find file version by ID", new { VersionId = id } );

    FileVersionDto result = _versionService.FindOne( id );
    if ( result == null )
    {
      LoggingHelper.LogFailure( _logger, "File version not found", new { VersionId = id } );
      return NotFound();
    }
    LoggingHelper.LogSuccess( _logger, "Returning file version", new { VersionId = id } );
    return Ok( result );
  }

  [HttpPatch( "{id}" )]
  public IActionResult Update( int id, [FromBody] UpdateFileVersionDto data )
  {
    LoggingHelper.LogRequest( _logger, "Update file version", new { VersionId = id } );

    FileVersionDto result = _versionService.Update( id, data );
    if ( result == null )
    {
      LoggingHelper.LogFailure( _logger, "File version not found", new { VersionId = id } );
      return NotFound();
    }
    LoggingHelper.LogSuccess( _logger, "File version updated successfully", new { VersionId = id } );
    return Ok( result );
  }

  [HttpDelete( "{id}" )]
  public IActionResult Remove( int id )
  {
    LoggingHelper.LogRequest( _logger, "Remove file version", new { VersionId = id } );

    bool success = _versionService.Remove( id );
    if ( !success )
    {
      LoggingHelper.LogFailure( _logger, "Failed to remove file version. Version not found", new { VersionId = id } );
      return NotFound();
    }
    LoggingHelper.LogSuccess( _logger, "File version removed successfully", new { VersionId = id } );
    return NoContent();
  }

  [HttpGet( "findByFileId/{fileId}" )]
  public IActionResult FindByFileId( int fileId )
  {
    LoggingHelper.LogRequest( _logger, "Find versions by file ID", new { FileId = fileId } );

    IEnumerable<FileVersionDto> result = _versionService.FindByFileId( fileId );
    return Ok( result );
  }
}
