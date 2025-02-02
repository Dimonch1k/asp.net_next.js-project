using backend_c_.Enums;

namespace backend_c_.Exceptions;

public class ServerException : Exception
{
  public ExceptionStatusCode StatusCode { get; set; }

  public ServerException( string message, ExceptionStatusCode statusCode ) : base( message )
  {
    StatusCode = statusCode;
  }
}
