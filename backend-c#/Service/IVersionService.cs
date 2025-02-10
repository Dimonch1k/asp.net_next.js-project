using backend_c_.DTO.Version;
using backend_c_.Entity;

namespace backend_c_.Service;

public interface IVersionService
{
  IEnumerable<FileVersionDto> GetAllVersions( );
  IEnumerable<FileVersionDto> GetVersionsByFileId( int fileId );
  Task<FileVersionDto> GetVersionById( int id );
  Task<FileVersionDto> CreateVersion( CreateFileVersionDto data, MediaFile file );
  FileVersionDto RestoreFileVersion( int versionId, MediaFile file, FileVersion fileVersion );
  Task<FileVersionDto> UpdateVersion( int id, UpdateFileVersionDto data );
  Task<FileVersionDto> DeleteVersion( int id );

  Task<FileVersion> GetFileVersionIfExists( int fileVersionId );
}
