namespace backend_c_.Exceptions.File;

public class MediaFileNotFoundException : AppException
{
  public MediaFileNotFoundException( int fileId )
      : base( $"File with ID {fileId} was not found." ) { }
}

