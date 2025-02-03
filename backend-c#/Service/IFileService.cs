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

  Task<FileDto> UploadFile( UploadFileDto data, IFormFile file );
  FileDto UpdateFile( int id, UpdateFileDto data );
  FileDto DeleteFile( int id );

  MediaFile GetFileIfExists( int fileId );
  void EnsureFileExists( int fileId );
  void EnsureFileIsNotNull( MediaFile? file );
}
