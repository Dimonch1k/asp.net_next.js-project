namespace backend_c_.Exceptions.File;

public class InvalidMediaFileTypeException : AppException
{
  public InvalidMediaFileTypeException( string fileType )
      : base( $"The file type '{fileType}' is not supported." ) { }
}
