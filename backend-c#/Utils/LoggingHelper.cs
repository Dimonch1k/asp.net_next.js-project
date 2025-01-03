namespace backend_c_.Utilities;

public static class LoggingHelper
{
  public static void LogRequest( ILogger logger, string action, object? data = null )
  {
    if ( data == null )
    {
      logger.LogInformation( "Received request to {Action}.", action );
      return;
    }
    logger.LogInformation( "Received request to {Action}. Data: {@Data}", action, data );
  }

  public static void LogSuccess( ILogger logger, string message, object? data = null )
  {
    if ( data == null )
    {
      logger.LogInformation( "{Message}.", message );
      return;
    }
    logger.LogInformation( "{Message}. Data: {@Data}", message, data );
  }

  public static void LogFailure( ILogger logger, string message, object? data = null )
  {
    if ( data == null )
    {
      logger.LogWarning( "{Message}.", message );
      return;
    }
    logger.LogWarning( "{Message}. Data: {@Data}", message, data );
  }
}
