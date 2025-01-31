using backend_c_.DTO.SharedFile;
using backend_c_.Entity;

namespace backend_c_.Service;

public interface ISharedFileService
{
  ShareFileDto Share( ShareFileDto data );
  ShareFileDto Remove( int id );

  void CheckIfSharedFileIsNull( SharedFile? sharedFile );
  ShareFileDto SharedFileToDto( SharedFile sharedFile );
}
