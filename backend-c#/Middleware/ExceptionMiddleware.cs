using System.Text.Json;
using backend_c_.Exceptions;
using Microsoft.AspNetCore.Http;

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
    catch ( ServerException ex )
    {
      _logger.LogError( ex, $"Exception: {ex.Message}; Type: {ex.StatusCode.ToString()}" );

      await HandleServerExceptionAsync( context, ex );
    }
    catch ( Exception ex )
    {
      _logger.LogError( ex, "Unhandled exception occurred." );

      await HandleExceptionAsync( context, ex );
    }
  }

  private Task HandleServerExceptionAsync( HttpContext context, ServerException exception )
  {

    int statusCode = (int) exception.StatusCode;

    object response = new
    {
      error = exception.Message,
      details = exception.InnerException?.Message, // remove after finishing work with Backend
    };

    context.Response.ContentType = "application/json";
    context.Response.StatusCode = statusCode;

    return context.Response.WriteAsync( JsonSerializer.Serialize( response ) );
  }

  private Task HandleExceptionAsync( HttpContext context, Exception exception )
  {
    object response = new
    {
      error = "Internal server error",
      details = exception.InnerException?.Message, // remove after finishing work with Backend
    };

    context.Response.ContentType = "application/json";
    context.Response.StatusCode = 500;

    return context.Response.WriteAsync( JsonSerializer.Serialize( response ) );
  }
}
