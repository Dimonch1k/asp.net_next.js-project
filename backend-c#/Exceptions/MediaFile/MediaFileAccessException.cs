namespace backend_c_.Exceptions.File;

public class MediaFileAccessException : AppException
{
  public MediaFileAccessException( int fileId, int userId )
      : base( $"User {userId} does not have access to file {fileId}." ) { }
}
