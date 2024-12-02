using backend_c_.DTO.Version;

namespace backend_c_.Services.Interfaces;

public interface IVersionService
{
  FileVersionDto Create( CreateFileVersionDto data );
  IEnumerable<FileVersionDto> FindAll( );
  FileVersionDto FindOne( int id );
  FileVersionDto Update( int id, UpdateFileVersionDto data );
  bool Remove( int id );
  IEnumerable<FileVersionDto> FindByFileId( int fileId );
}
