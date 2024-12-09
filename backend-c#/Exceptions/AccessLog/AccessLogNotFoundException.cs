namespace backend_c_.Exceptions.AccessLog;

public class AccessLogNotFoundException : AppException
{
  public AccessLogNotFoundException( int logId )
      : base( $"Access log with ID {logId} was not found." ) { }
}

