using backend_c_.DTO.SharedFile;
using backend_c_.Entity;

namespace backend_c_.Service;

public interface ISharedFileService
{
  ShareFileDto ShareFile( ShareFileDto data );
  ShareFileDto DeleteSharedFile( int id );

  void EnsureSharedFileIsNotNull( SharedFile? sharedFile );
  ShareFileDto SharedFileToDto( SharedFile sharedFile );
}
