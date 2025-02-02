using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.Access;
using backend_c_.Service;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace backend_c_.Controllers;

[ApiController]
[Route( "api/v1/[controller]" )]
[Authorize]
public class AccessLogsController : ControllerBase
{
  private readonly IAccessLogService _accessLogService;
  private readonly ILogger<AccessLogsController> _logger;

  public AccessLogsController( IAccessLogService accessLogService, ILogger<AccessLogsController> logger )
  {
    _accessLogService = accessLogService;
    _logger = logger;
  }

  [HttpGet]
  public IActionResult FindAll( )
  {
    _logger.LogInformation( "find all access logs." );

    IEnumerable<AccessLogDto> accessLogsDto = _accessLogService.FindAll();

    _logger.LogInformation( "Returning {AccessLogCount} access logs.", accessLogsDto.Count() );

    return Ok( accessLogsDto );
  }

  [HttpGet( "{id}" )]
  public IActionResult FindOne( int id )
  {
    _logger.LogInformation( "find access log with ID: {AccessLogId}", id );

    AccessLogDto accessLogDto = _accessLogService.FindOne( id );

    _logger.LogInformation( "Returning access log with ID: {AccessLogId}.", id );

    return Ok( accessLogDto );
  }

  [HttpPost]
  public IActionResult Create( [FromBody] CreateAccessLogDto createAccessLogDto )
  {
    _logger.LogInformation( "create access log for file: {FileId} by user: {UserId}.", createAccessLogDto.FileId, createAccessLogDto.UserId );

    AccessLogDto accessLogDto = _accessLogService.Create( createAccessLogDto );

    _logger.LogInformation( "Access log created successfully with ID: {AccessLogId}.", accessLogDto.Id );

    return CreatedAtAction(
      nameof( FindOne ),
      new { id = accessLogDto.Id },
      accessLogDto
    );
  }

  [HttpPatch( "{id}" )]
  public IActionResult Update( int id, [FromBody] UpdateAccessLogDto updateAccessLogDto )
  {
    _logger.LogInformation( "Received request to update access log with ID: {AccessLogId}.", id );

    AccessLogDto accessLogDto = _accessLogService.Update( id, updateAccessLogDto );

    _logger.LogInformation( "Access log with ID {AccessLogId} updated successfully.", id );

    return Ok( accessLogDto );
  }

  [HttpDelete( "{id}" )]
  public IActionResult Remove( int id )
  {
    _logger.LogInformation( "Received request to remove access log with ID: {AccessLogId}.", id );

    AccessLogDto removedAccessLogDto = _accessLogService.Remove( id );

    _logger.LogInformation( "Access log with ID {AccessLogId} removed successfully.", id );

    return Ok(removedAccessLogDto);
  }

  [HttpGet( "file/{fileId}" )]
  public IActionResult FindAccessByFile( int fileId )
  {
    _logger.LogInformation( "Received request to find access logs for file with ID: {FileId}.", fileId );

    return Ok( _accessLogService.FindAccessByFile( fileId ) );
  }

  [HttpGet( "user/{userId}" )]
  public IActionResult FindAccessByUser( int userId )
  {
    _logger.LogInformation( "Received request to find access logs for user with ID: {UserId}.", userId );

    return Ok( _accessLogService.FindAccessByUser( userId ) );
  }
}
