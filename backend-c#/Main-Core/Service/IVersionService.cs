using backend_c_.DTO.Version;

namespace backend_c_.Service;

public interface IVersionService
{
  FileVersionDto Create( CreateFileVersionDto data );
  FileVersionDto RestoreVersion( int versionId );
  IEnumerable<FileVersionDto> FindAll( );
  FileVersionDto FindOne( int id );
  FileVersionDto Update( int id, UpdateFileVersionDto data );
  FileVersionDto Remove( int id );
  IEnumerable<FileVersionDto> FindByFileId( int fileId );

  void CheckByPathIfFileExists( string path );
}
