using backend_c_.DTO.File;
using backend_c_.DTO.SharedFile;
using backend_c_.Entity;
using backend_c_.Enums;
using backend_c_.Exceptions;
using backend_c_.Utilities;
using backend_c_.Utils;
using Microsoft.EntityFrameworkCore;

namespace backend_c_.Service.Impl;

public class MediaFileService : IFileService
{
  private readonly AppDbContext _dbContext;
  private readonly Lazy<IUserService> _userService;
  private readonly Lazy<ISharedFileService> _sharedFileService;
  private readonly Lazy<IVersionService> _versionService;
  private readonly TimeZoneHelper _timeZoneHelper;
  private readonly IConfiguration _configuration;
  private readonly ILogger<MediaFileService> _logger;

  public MediaFileService( AppDbContext dbContext, Lazy<IUserService> userService, Lazy<ISharedFileService> sharedFileService, Lazy<IVersionService> versionService, TimeZoneHelper timeZoneHelper, IConfiguration configuration, ILogger<MediaFileService> logger )
  {
    _dbContext = dbContext;
    _userService = userService;
    _sharedFileService = sharedFileService;
    _versionService = versionService;
    _timeZoneHelper = timeZoneHelper;
    _configuration = configuration;
    _logger = logger;
  }

  public async Task<IEnumerable<ShareFileDto>> GetFilesSharedByMe( int userId )
  {
    await _userService.Value.EnsureUserExists( userId );

    return _dbContext.SharedFiles
      .Where( sf => sf.OwnerId == userId )
      .Select( _sharedFileService.Value.SharedFileToDto )
      .ToList();
  }

  public async Task<IEnumerable<ShareFileDto>> GetFilesSharedToMe( int userId )
  {
    await _userService.Value.EnsureUserExists( userId );

    return _dbContext.SharedFiles
      .Where( sf => sf.SharedWithId == userId )
      .Select( _sharedFileService.Value.SharedFileToDto )
      .ToList();
  }

  public async Task<IEnumerable<FileDto>> GetUserFiles( int userId )
  {
    await _userService.Value.EnsureUserExists( userId );

    return _dbContext.Files
      .Where( file => file.UserId == userId )
      .Select( FileToDto )
      .ToList();
  }

  public async Task<FileDto> UploadFile( UploadFileDto data, IFormFile uploadedFile )
  {
    await _userService.Value.EnsureUserExists( data.UserId );

    EnsureFileIsUnique( uploadedFile.FileName, data.UserId );

    PathHelper.EnsureDirectoryExists( PathHelper._tempFolder );

    ValidationHelpers.EnsureFileHasValidFileType( uploadedFile.ContentType );

    string tempFilePath = Path.Combine( PathHelper._tempFolder, Guid.NewGuid() + "_" + uploadedFile.FileName );

    using ( FileStream stream = new FileStream( tempFilePath, FileMode.Create ) )
    {
      await uploadedFile.CopyToAsync( stream );
    }

    FileScanRequest scanRequest = new FileScanRequest
    {
      FileId = Guid.NewGuid(),
      FilePath = tempFilePath,
      FileName = uploadedFile.FileName,
      Status = RequestStatus.pending.ToString(),
      CreatedAt = DateTime.UtcNow
    };

    _dbContext.FileScanRequests.Add( scanRequest );
    await _dbContext.SaveChangesAsync();

    FileScanRequest? updatedRequest = await WaitForScanResult( scanRequest.FileId, TimeSpan.FromSeconds( 90 ) );

    if ( updatedRequest == null )
    {
      _logger.LogError( "Scan request not found" );

      throw new ServerException( "Scan request not found", ExceptionStatusCode.ScanResultNotFound );
    }

    return await ProcessScanResult( updatedRequest, data, uploadedFile.FileName, uploadedFile.ContentType, tempFilePath );
  }

  private async Task<FileScanRequest?> WaitForScanResult( Guid fileId, TimeSpan timeout )
  {
    DateTime startTime = DateTime.UtcNow;

    while ( DateTime.UtcNow - startTime < timeout )
    {
      await Task.Delay( 1000 );
      FileScanRequest? fileScanRequest = await _dbContext.FileScanRequests
        .AsNoTracking()
        .FirstOrDefaultAsync( fsr =>
          fsr.FileId.Equals( fileId ) &&
          ( fsr.Status.ToLower() == RequestStatus.clean.ToString()
          || fsr.Status.ToLower() == RequestStatus.infected.ToString() )
        );

      if ( fileScanRequest != null )
      {
        return fileScanRequest;
      }
    }
    return null;
  }

  private async Task<FileDto> ProcessScanResult( FileScanRequest request, UploadFileDto data, string fileName, string contentType, string tempFilePath )
  {
    if ( request.Status.ToLower() == RequestStatus.clean.ToString() )
    {
      string filePath = SaveFile( data, fileName, contentType );

      MediaFile newFile = new MediaFile
      {
        UserId = data.UserId,
        FileName = fileName,
        FilePath = filePath,
        FileSize = data.FileData.Length,
        FileType = contentType,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      _dbContext.Files.Add( newFile );
      await _dbContext.SaveChangesAsync();

      return FileToDto( newFile );
    }

    if ( request.Status.ToLower() == RequestStatus.infected.ToString() )
    {
      if ( System.IO.File.Exists( tempFilePath ) )
      {
        System.IO.File.Delete( tempFilePath );
      }

      _logger.LogError( "File contains a virus and cannot be uploaded." );

      throw new ServerException( "File contains a virus and cannot be uploaded.", ExceptionStatusCode.FileInfected );
    }

    _logger.LogError( "Virus scan timeout. Please try again later." );

    throw new ServerException( "Virus scan timeout. Please try again later.", ExceptionStatusCode.ScanTimeout );
  }


  public async Task<FileDto> UpdateFile( int id, UpdateFileDto data )
  {
    MediaFile? file = await GetFileIfExists( id );

    EnsureFileIsUnique( data.FileName, file.UserId );

    string oldFilePath = file.FilePath;
    string newFilePath = PathHelper.UpdatePath( file.FilePath, data.FileName );

    PathHelper.EnsureFileExistsAtPath( oldFilePath );

    // File path name change, not moving file to new path
    File.Move( oldFilePath, newFilePath );

    file.FileName = data.FileName;
    file.FilePath = newFilePath;
    file.UpdatedAt = DateTime.UtcNow;

    _dbContext.Files.Update( file );
    _dbContext.SaveChanges();

    return FileToDto( file );
  }

  public async Task<FileDto> DeleteFile( int id )
  {
    MediaFile? file = await GetFileIfExists( id );

    file.DeletedAt = DateTime.UtcNow;

    _dbContext.Files.Remove( file );
    _dbContext.SaveChanges();

    return FileToDto( file );
  }


  private string SaveFile( UploadFileDto data, string fileName, string fileType )
  {
    if ( data == null || data.FileData == null || string.IsNullOrEmpty( fileName ) )
    {
      _logger.LogError( "Invalid file upload data" );

      throw new ServerException( "Invalid file upload data", ExceptionStatusCode.BadRequest );
    }

    if ( !ValidationHelpers.BeAValidFileType( fileType ) )
    {
      _logger.LogError( "Invalid file type" );

      throw new ServerException( "Invalid file type", ExceptionStatusCode.UnsupportedMediaType );
    }

    string filePath = PathHelper.GetFilePath( data.UserId, fileName );

    try
    {
      File.WriteAllBytes( filePath, data.FileData );
    }
    catch ( IOException )
    {
      _logger.LogError( "Failed to save file due to storage issues." );

      throw new ServerException( "Failed to save file due to storage issues.", ExceptionStatusCode.InsufficientStorage );
    }
    catch ( UnauthorizedAccessException )
    {
      _logger.LogError( "Insufficient permissions to save file." );

      throw new ServerException( "Insufficient permissions to save file.", ExceptionStatusCode.Forbidden );
    }

    return filePath;
  }

  public async Task<MediaFile> GetFileIfExists( int? fileId )
  {
    MediaFile? file = await _dbContext.Files.FindAsync( fileId );

    if ( file == null )
    {
      _logger.LogError( "File not found" );

      throw new ServerException( $"File with ID='{fileId}' not found", ExceptionStatusCode.FileNotFound );
    }

    return file;
  }

  public async Task EnsureFileExists( int? fileId )
  {
    if ( await _dbContext.Files.FindAsync( fileId ) == null )
    {
      _logger.LogError( "File not found" );

      throw new ServerException( $"File with ID='{fileId}' not found", ExceptionStatusCode.FileNotFound );
    }
  }

  public async Task EnsureFileBelongsToSenderNotReceiver( int? fileId, int? ownerId, int? sharedWithId )
  {
    MediaFile? file = await GetFileIfExists( fileId );

    if ( file.UserId != ownerId )
    {
      _logger.LogError( $"The file with ID: '{fileId}' doesn't belong to user with ID: '{ownerId}'" );

      throw new ServerException( $"The file with ID: '{fileId}' doesn't belong to user with ID: '{ownerId}'", ExceptionStatusCode.BadRequest );
    }

    if ( file.UserId == sharedWithId )
    {
      _logger.LogError( $"The file with ID: '{fileId}' can't be sent by user with ID: '{ownerId}' to the receiver with ID: '{sharedWithId}'" );

      throw new ServerException( $"The file with ID: '{fileId}' can't be sent by user with ID: '{ownerId}' to the receiver with ID: '{sharedWithId}'", ExceptionStatusCode.BadRequest );
    }
  }

  private void EnsureFileIsUnique( string fileName, int userId )
  {
    MediaFile? file = _dbContext.Files.FirstOrDefault(
     f => f.FileName.Equals( fileName )
       && f.UserId.Equals( userId )
   );

    if ( file != null )
    {
      _logger.LogError( $"File with Name: '{file.FileName}' already exists" );

      throw new ServerException( $"File with Name: '{file.FileName}' already exists", ExceptionStatusCode.FileDuplicate );
    }
  }

  private FileDto FileToDto( MediaFile file )
  {
    User? user = _dbContext.Users.Find( file.UserId );

    return new FileDto
    {
      Id = file.Id,
      UserId = file.UserId,
      FileName = file.FileName,
      FilePath = file.FilePath,
      FileSize = file.FileSize,
      FileType = file.FileType,
      CreatedAt = file.CreatedAt,
      UpdatedAt = file.UpdatedAt,
      DeletedAt = file.DeletedAt,
      CreatedAtFormatted = _timeZoneHelper.GetHumanReadableTime( file.CreatedAt, user.TimeZoneId ),
      UpdatedAtFormatted = _timeZoneHelper.GetHumanReadableTime( file.UpdatedAt, user.TimeZoneId ),
    };
  }
}
