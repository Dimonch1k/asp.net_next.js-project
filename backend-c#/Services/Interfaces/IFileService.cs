using backend_c_.DTO.File;

namespace backend_c_.Services.Interfaces;

public interface IFileService
{
  FileDto Upload( UploadFileDto data );
  FileDto Update( int id, UpdateFileDto data );
  bool Remove( int id );
  IEnumerable<FileDto> GetUserFiles( int userId );
  IEnumerable<FileDto> GetFilesSharedToMe( int userId );
  IEnumerable<FileDto> GetFilesSharedByMe( int userId );
}
