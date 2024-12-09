using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.Version;
using backend_c_.Service;
using Microsoft.Extensions.Logging;

namespace backend_c_.Controllers
{
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

    [HttpPost( "create" )]
    public IActionResult Create( [FromBody] CreateFileVersionDto data )
    {
      _logger.LogInformation( "Received request to create file version for file with ID: {FileId}.", data.FileId );

      FileVersionDto result = _versionService.Create( data );
      _logger.LogInformation( "File version created successfully with ID: {VersionId}.", result.Id );
      return CreatedAtAction( nameof( FindOne ), new { id = result.Id }, result );
    }

    [HttpGet( "findAll" )]
    public IActionResult FindAll( )
    {
      _logger.LogInformation( "Received request to find all file versions." );

      IEnumerable<FileVersionDto> result = _versionService.FindAll();
      _logger.LogInformation( "Returning {FileVersionCount} file versions.", result.Count() );
      return Ok( result );
    }

    [HttpGet( "findOne/{id}" )]
    public IActionResult FindOne( int id )
    {
      _logger.LogInformation( "Received request to find file version with ID: {VersionId}", id );

      FileVersionDto result = _versionService.FindOne( id );
      if ( result == null )
      {
        _logger.LogWarning( "File version with ID {VersionId} not found.", id );
        return NotFound();
      }
      _logger.LogInformation( "Returning file version with ID: {VersionId}.", id );
      return Ok( result );
    }

    [HttpPatch( "update/{id}" )]
    public IActionResult Update( int id, [FromBody] UpdateFileVersionDto data )
    {
      _logger.LogInformation( "Received request to update file version with ID: {VersionId}.", id );

      FileVersionDto result = _versionService.Update( id, data );
      if ( result == null )
      {
        _logger.LogWarning( "File version with ID {VersionId} not found.", id );
        return NotFound();
      }
      _logger.LogInformation( "File version with ID {VersionId} updated successfully.", id );
      return Ok( result );
    }

    [HttpDelete( "remove/{id}" )]
    public IActionResult Remove( int id )
    {
      _logger.LogInformation( "Received request to remove file version with ID: {VersionId}.", id );

      bool success = _versionService.Remove( id );
      if ( !success )
      {
        _logger.LogWarning( "Failed to remove file version with ID {VersionId}. File version not found.", id );
        return NotFound();
      }
      _logger.LogInformation( "File version with ID {VersionId} removed successfully.", id );
      return NoContent();
    }

    [HttpGet( "findByFileId/{fileId}" )]
    public IActionResult FindByFileId( int fileId )
    {
      _logger.LogInformation( "Received request to find versions for file with ID: {FileId}.", fileId );

      IEnumerable<FileVersionDto> result = _versionService.FindByFileId( fileId );
      return Ok( result );
    }
  }
}
