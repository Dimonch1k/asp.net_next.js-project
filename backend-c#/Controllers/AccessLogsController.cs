using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.Access;
using backend_c_.Service;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace backend_c_.Controllers;

[ApiController]
[Route( "api/v1/[controller]" )]
[Authorize]
public class AccessController : ControllerBase
{
  private readonly IAccessLogService _accessLogService;
  private readonly ILogger<AccessController> _logger;

  public AccessController( IAccessLogService accessLogService, ILogger<AccessController> logger )
  {
    _accessLogService = accessLogService;
    _logger = logger;
  }

  // Get All Access Logs
  [HttpGet]
  public IActionResult FindAll( )
  {
    _logger.LogInformation( "Received request to find all access logs." );

    IEnumerable<AccessLogDto> result = _accessLogService.FindAll();

    _logger.LogInformation( "Returning {AccessLogCount} access logs.", result.Count() );

    return Ok( result );
  }

  // Get Access Log by ID
  [HttpGet( "{id}" )]
  public IActionResult FindOne( int id )
  {
    _logger.LogInformation( "Received request to find access log with ID: {AccessLogId}", id );

    AccessLogDto result = _accessLogService.FindOne( id );
    if ( result == null )
    {
      _logger.LogWarning( "Access log with ID {AccessLogId} not found.", id );
      return NotFound();
    }
    _logger.LogInformation( "Returning access log with ID: {AccessLogId}.", id );
    return Ok( result );
  }

  // Create Access Log
  [HttpPost]
  public IActionResult Create( [FromBody] CreateAccessLogDto createAccessLogDto )
  {
    _logger.LogInformation( "Received request to create access log for file: {FileId} by user: {UserId}.", createAccessLogDto.FileId, createAccessLogDto.UserId );

    AccessLogDto result = _accessLogService.Create( createAccessLogDto );

    _logger.LogInformation( "Access log created successfully with ID: {AccessLogId}.", result.Id );
    return CreatedAtAction(
      nameof( FindOne ),
      new { id = result.Id },
      result
    );
  }

  // Update Access Log by ID
  [HttpPatch( "{id}" )]
  public IActionResult Update( int id, [FromBody] UpdateAccessLogDto updateAccessLogDto )
  {
    _logger.LogInformation( "Received request to update access log with ID: {AccessLogId}.", id );

    AccessLogDto result = _accessLogService.Update( id, updateAccessLogDto );
    if ( result == null )
    {
      _logger.LogWarning( "Access log with ID {AccessLogId} not found.", id );
      return NotFound();
    }
    _logger.LogInformation( "Access log with ID {AccessLogId} updated successfully.", id );
    return Ok( result );
  }

  // Delete Access Log by ID
  [HttpDelete( "{id}" )]
  public IActionResult Remove( int id )
  {
    _logger.LogInformation( "Received request to remove access log with ID: {AccessLogId}.", id );

    bool success = _accessLogService.Remove( id );
    if ( !success )
    {
      _logger.LogWarning( "Failed to remove access log with ID {AccessLogId}. Access log not found.", id );
      return NotFound();
    }
    _logger.LogInformation( "Access log with ID {AccessLogId} removed successfully.", id );
    return NoContent();
  }

  // Get All Access Logs by File ID
  [HttpGet( "file/{fileId}" )]
  public IActionResult FindAccessByFile( int fileId )
  {
    _logger.LogInformation( "Received request to find access logs for file with ID: {FileId}.", fileId );

    IEnumerable<AccessLogDto> result = _accessLogService.FindAccessByFile( fileId );
    return Ok( result );
  }

  // Get All Access Logs by User ID
  [HttpGet( "user/{userId}" )]
  public IActionResult FindAccessByUser( int userId )
  {
    _logger.LogInformation( "Received request to find access logs for user with ID: {UserId}.", userId );

    IEnumerable<AccessLogDto> result = _accessLogService.FindAccessByUser( userId );
    return Ok( result );
  }
}
