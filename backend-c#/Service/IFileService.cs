using backend_c_.DTO.File;
using backend_c_.DTO.SharedFile;
using backend_c_.Entity;
using Microsoft.AspNetCore.Mvc;

namespace backend_c_.Service;

public interface IFileService
{
  Task<IEnumerable<ShareFileDto>> GetFilesSharedByMe( int userId );
  Task<IEnumerable<ShareFileDto>> GetFilesSharedToMe( int userId );
  Task<IEnumerable<FileDto>> GetUserFiles( int userId );

  Task<FileDto> UploadFile( UploadFileDto data, IFormFile file );
  Task<FileDto> UpdateFile( int id, UpdateFileDto data );
  Task<FileDto> DeleteFile( int id );

  Task<MediaFile> GetFileIfExists( int? fileId );
  Task EnsureFileExists( int? fileId );
  Task EnsureFileBelongsToSenderNotReceiver( int? fileId, int? ownerId, int? sharedWithId );
}
