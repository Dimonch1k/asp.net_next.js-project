namespace backend_c_.Exceptions;

public class ConflictException : AppException
{
  public ConflictException( string message )
      : base( $"Conflict: {message}" ) { }
}

