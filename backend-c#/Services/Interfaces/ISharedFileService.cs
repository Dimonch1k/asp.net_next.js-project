using backend_c_.DTO.SharedFile;

namespace backend_c_.Services.Interfaces;

public interface ISharedFileService
{
  ShareFileDto Share( ShareFileDto data );
  bool Remove( int id );
}
