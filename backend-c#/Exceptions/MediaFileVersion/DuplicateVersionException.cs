namespace backend_c_.Exceptions.FileVersion;

public class DuplicateVersionException : AppException
{
  public DuplicateVersionException( string versionName )
      : base( $"File version '{versionName}' already exists." ) { }
}

