using backend_c_.DTO.SharedFile;

namespace backend_c_.Service;

public interface ISharedFileService
{
    ShareFileDto Share(ShareFileDto data);
    ShareFileDto Remove(int id);
}
