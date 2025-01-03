namespace backend_c_.Exceptions.SharedFile;

public class SharedFileNotFoundException : AppException
{
  public SharedFileNotFoundException( int sharedFileId )
      : base( $"Shared file with ID {sharedFileId} was not found." ) { }
}

