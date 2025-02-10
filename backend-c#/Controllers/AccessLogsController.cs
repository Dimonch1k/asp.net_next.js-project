using Microsoft.AspNetCore.Mvc;
using backend_c_.DTO.Access;
using backend_c_.Service;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using backend_c_.Entity;

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
  public IActionResult GetAllAccessLogs( )
  {
    _logger.LogInformation( "Received request to get all access logs." );

    IEnumerable<AccessLogDto> accessLogsDto = _accessLogService.GetAllAccessLogs();

    _logger.LogInformation( $"Returning {accessLogsDto.Count()} access logs." );

    return Ok( accessLogsDto );
  }

  [HttpGet( "{id}" )]
  public async Task<IActionResult> GetAccessLogById( int id )
  {
    _logger.LogInformation( $"Received request to get access log with ID: {id}" );

    AccessLogDto accessLogDto = await _accessLogService.GetAccessLogById( id );

    _logger.LogInformation( $"Returning access log with ID: {id}." );

    return Ok( accessLogDto );
  }

  [HttpGet( "file-id/{fileId}" )]
  public async Task<IActionResult> GetAccessLogsByFileId( int fileId )
  {
    _logger.LogInformation( $"Received request to get access logs for file with ID: {fileId}." );

    IEnumerable<AccessLogDto> accessLogsDto = await _accessLogService.GetAccessLogsBySharedFileId( fileId );

    _logger.LogInformation( "Returning access logs" );

    return Ok( accessLogsDto );
  }

  [HttpGet( "user-id/{userId}" )]
  public async Task<IActionResult> GetAccessLogsByUserId( int userId )
  {
    _logger.LogInformation( $"Received request to get access logs for user with ID: {userId}." );

    IEnumerable<AccessLogDto> accessLogsDto = await _accessLogService.GetAccessLogsByUserId( userId );

    _logger.LogInformation( "Returning access logs" );

    return Ok( accessLogsDto );
  }

  [HttpPost]
  public async Task<IActionResult> CreateAccessLog( [FromBody] CreateAccessLogDto createAccessLogDto )
  {
    _logger.LogInformation( $"Received request to create access log for shared file with ID: {createAccessLogDto.SharedFileId} by user with ID: {createAccessLogDto.UserId}." );

    AccessLogDto accessLogDto = await _accessLogService.CreateAccessLog( createAccessLogDto );

    _logger.LogInformation( $"Access log created successfully with ID: {accessLogDto.Id}." );

    return Ok( accessLogDto );
  }

  [HttpPatch( "{id}" )]
  public async Task<IActionResult> UpdateAccessLog( int id, [FromBody] UpdateAccessLogDto updateAccessLogDto )
  {
    _logger.LogInformation( $"Received request to update access log with ID: {id}." );

    AccessLogDto accessLogDto = await _accessLogService.UpdateAccessLog( id, updateAccessLogDto );

    _logger.LogInformation( $"Access log with ID: {id} updated successfully." );

    return Ok( accessLogDto );
  }

  [HttpDelete( "{id}" )]
  public async Task<IActionResult> DeleteAccessLog( int id )
  {
    _logger.LogInformation( $"Received request to delete access log with ID: {id}." );

    AccessLogDto deletedAccessLogDto = await _accessLogService.DeleteAccessLog( id );

    _logger.LogInformation( $"Access log with ID: {id} deleted successfully." );

    return Ok( deletedAccessLogDto );
  }
}
