using backend_c_.DTO.Version;
using backend_c_.Entity;

namespace backend_c_.Service;

public interface IVersionService
{
  IEnumerable<FileVersionDto> GetAllVersions( );
  IEnumerable<FileVersionDto> GetVersionsByFileId( int fileId );
  FileVersionDto GetVersionById( int id );
  FileVersionDto CreateVersion( CreateFileVersionDto data, MediaFile file );
  FileVersionDto RestoreFileVersion( int versionId, MediaFile file, FileVersion fileVersion );
  FileVersionDto UpdateVersion( int id, UpdateFileVersionDto data );
  FileVersionDto DeleteVersion( int id );

  public FileVersion GetFileVersionIfExists( int fileVersionId );
  void EnsureFileExistsAtPath( string path );
}
