using backend_c_.DTO.File;
using backend_c_.DTO.SharedFile;
using backend_c_.Entity;
using Microsoft.AspNetCore.Mvc;

namespace backend_c_.Service;

public interface IFileService
{
  IEnumerable<ShareFileDto> GetFilesSharedByMe( int userId );
  IEnumerable<ShareFileDto> GetFilesSharedToMe( int userId );
  IEnumerable<FileDto> GetUserFiles( int userId );

  Task<FileDto> Upload( UploadFileDto data, IFormFile file );
  FileDto Update( int id, UpdateFileDto data );
  FileDto Remove( int id );

  MediaFile CheckIfFileExists( int fileId );
  void CheckIfFileIsNull( MediaFile? file );
}
