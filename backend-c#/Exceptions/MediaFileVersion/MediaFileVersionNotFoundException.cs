namespace backend_c_.Exceptions.FileVersion;

public class MediaFileVersionNotFoundException : AppException
{
  public MediaFileVersionNotFoundException( int versionId )
      : base( $"File version with ID {versionId} was not found." ) { }
}

