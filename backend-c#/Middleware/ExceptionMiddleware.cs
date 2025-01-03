using Microsoft.AspNetCore.Http;
using System.Text.Json;
using backend_c_.Exceptions;
using backend_c_.Exceptions.AccessLog;
using backend_c_.Exceptions.File;
using backend_c_.Exceptions.FileVersion;
using backend_c_.Exceptions.SharedFile;
using backend_c_.Exceptions.User;

public class ExceptionMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ExceptionMiddleware> _logger;

  public ExceptionMiddleware( RequestDelegate next, ILogger<ExceptionMiddleware> logger )
  {
    _next = next;
    _logger = logger;
  }

  public async Task Invoke( HttpContext context )
  {
    try
    {
      await _next( context );
    }
    catch ( Exception ex )
    {
      _logger.LogError( ex, "Unhandled exception occurred." );
      await HandleExceptionAsync( context, ex );
    }
  }

  private Task HandleExceptionAsync( HttpContext context, Exception exception )
  {
    int statusCode = exception switch
    {
      InvalidMediaFileTypeException => StatusCodes.Status400BadRequest,
      FileNotFoundException => StatusCodes.Status404NotFound,
      MediaFileAccessException => StatusCodes.Status403Forbidden,
      AccessLogNotFoundException => StatusCodes.Status404NotFound,
      SharedFilePermissionException => StatusCodes.Status403Forbidden,
      SharedFileNotFoundException => StatusCodes.Status404NotFound,
      MediaFileVersionNotFoundException => StatusCodes.Status404NotFound,
      DuplicateVersionException => StatusCodes.Status409Conflict,
      ConflictException => StatusCodes.Status409Conflict,
      UserNotFoundException => StatusCodes.Status404NotFound,
      DuplicateUserException => StatusCodes.Status409Conflict,
      UnauthorizedOperationException => StatusCodes.Status401Unauthorized,
      FluentValidation.ValidationException => StatusCodes.Status400BadRequest,
      _ => StatusCodes.Status500InternalServerError
    };

    object response = new
    {
      error = exception.Message,
      details = exception.InnerException?.Message
    };

    context.Response.ContentType = "application/json";
    context.Response.StatusCode = statusCode;

    return context.Response.WriteAsync( JsonSerializer.Serialize( response ) );
  }
}
