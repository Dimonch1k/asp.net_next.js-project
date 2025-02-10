using backend_c_.DTO.SharedFile;
using backend_c_.Entity;

namespace backend_c_.Service;

public interface ISharedFileService
{
  IEnumerable<ShareFileDto> GetAllSharedFiles( );
  IEnumerable<ShareFileDto> GetMySharedFiles( int ownerId );
  ShareFileDto ShareFile( ShareFileDto data );
  Task<ShareFileDto> DeleteSharedFile( int? id );

  Task<SharedFile> GetSharedFileIfExists( int? sharedFileId );
  ShareFileDto SharedFileToDto( SharedFile sharedFile );
}
