namespace backend_c_.Exceptions.SharedFile;

public class SharedFilePermissionException : AppException
{
  public SharedFilePermissionException( int userId )
      : base( $"User {userId} does not have permission to share this file." ) { }
}

