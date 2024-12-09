namespace backend_c_.Exceptions;

public class ValidationException : AppException
{
  public ValidationException( string message )
      : base( $"Validation failed: {message}" ) { }
}

