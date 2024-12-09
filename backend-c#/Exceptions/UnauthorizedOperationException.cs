namespace backend_c_.Exceptions;

public class UnauthorizedOperationException : AppException
{
  public UnauthorizedOperationException( )
      : base( "You are not authorized to perform this operation." ) { }
}

