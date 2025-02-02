using backend_c_.DTO.File;
using backend_c_.DTO.SharedFile;
using backend_c_.DTO.User;
using backend_c_.Entity;
using backend_c_.Enums;
using backend_c_.Exceptions;
using backend_c_.Utilities;
using backend_c_.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend_c_.Service.Impl;

public class MediaFileService : IFileService
{
  private readonly AppDbContext _dbContext;
  private readonly Lazy<IUserService> _userService;
  private readonly Lazy<ISharedFileService> _sharedFileService;
  private readonly Lazy<IVersionService> _versionService;
  private readonly IConfiguration _configuration;
  private readonly ILogger<MediaFileService> _logger;

  public MediaFileService( AppDbContext dbContext, Lazy<IUserService> userService, Lazy<ISharedFileService> sharedFileService, Lazy<IVersionService> versionService, IConfiguration configuration, ILogger<MediaFileService> logger )
  {
    _dbContext = dbContext;
    _userService = userService;
    _sharedFileService = sharedFileService;
    _versionService = versionService;
    _configuration = configuration;
    _logger = logger;
  }

  public IEnumerable<ShareFileDto> GetFilesSharedByMe( int userId )
  {
    _userService.Value.CheckIfUserExists( userId );

    return _dbContext.SharedFiles
      .Where( sf => sf.OwnerId == userId )
      .Select( sf => _sharedFileService.Value.SharedFileToDto( sf ) )
      .ToList();
  }

  public IEnumerable<ShareFileDto> GetFilesSharedToMe( int userId )
  {
    _userService.Value.CheckIfUserExists( userId );

    return _dbContext.SharedFiles
      .Where( sf => sf.SharedWithId == userId )
      .Select( sf => _sharedFileService.Value.SharedFileToDto( sf ) )
      .ToList();
  }

  public IEnumerable<FileDto> GetUserFiles( int userId )
  {
    _userService.Value.CheckIfUserExists( userId );

    return _dbContext.Files
      .Where( file => file.UserId == userId )
      .Select( file => FileToDto( file ) )
      .ToList();
  }

  public async Task<FileDto> Upload( UploadFileDto data, IFormFile uploadedFile )
  {
    _userService.Value.CheckIfUserExists( data.UserId );

    CheckFileDuplicate( uploadedFile.FileName, data.UserId );

    PathHelper.EnsureDirectoryExists( PathHelper.tempFolder );

    string tempFilePath = Path.Combine( PathHelper.tempFolder, Guid.NewGuid() + "_" + uploadedFile.FileName );

    using ( var stream = new FileStream( tempFilePath, FileMode.Create ) )
    {
      await uploadedFile.CopyToAsync( stream );
    }

    FileScanRequest scanRequest = new()
    {
      FileId = Guid.NewGuid(),
      FilePath = tempFilePath,
      FileName = uploadedFile.FileName,
      Status = "Pending",
      CreatedAt = DateTime.UtcNow
    };

    _dbContext.FileScanRequests.Add( scanRequest );
    await _dbContext.SaveChangesAsync();

    FileScanRequest? updatedRequest = await WaitForScanResult( scanRequest.FileId, TimeSpan.FromSeconds( 60 ) );

    if ( updatedRequest == null )
    {
      LoggingHelper.LogFailure( _logger, "Scan request is NULL" );

      throw new ServerException( "Scan request is NULL", ExceptionStatusCode.InternalServerError );
    }

    if ( updatedRequest.Status.Equals( "Clean" ) )
    {
      string filePath = SaveFile( data, uploadedFile.FileName, uploadedFile.ContentType );

      MediaFile newFile = new()
      {
        UserId = data.UserId,
        FileName = uploadedFile.FileName,
        FilePath = filePath,
        FileSize = data.FileData.Length,
        FileType = uploadedFile.ContentType,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      _dbContext.Files.Add( newFile );
      await _dbContext.SaveChangesAsync();

      return FileToDto( newFile );
    }

    if ( updatedRequest.Status.Equals( "Infected" ) )
    {
      if ( System.IO.File.Exists( tempFilePath ) )
      {
        System.IO.File.Delete( tempFilePath );
      }

      LoggingHelper.LogFailure( _logger, "File contains a virus and cannot be uploaded." );

      throw new ServerException( "File contains a virus and cannot be uploaded.", ExceptionStatusCode.FileInfected );
    }

    LoggingHelper.LogFailure( _logger, "Virus scan timeout. Please try again later." );

    throw new ServerException( "Virus scan timeout. Please try again later.", ExceptionStatusCode.ScanTimeout );
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
          fsr.FileId.Equals( fileId )
          && ( fsr.Status.Equals( "Clean" )
            || fsr.Status.Equals( "Infected" ) )
        );

      if ( fileScanRequest != null )
      {
        return fileScanRequest;
      }
    }
    return null;
  }


  public FileDto Update( int id, UpdateFileDto data )
  {
    MediaFile? file = CheckIfFileExists( id );

    string oldFilePath = file.FilePath;
    string newFilePath = PathHelper.UpdatePath( file.FilePath, data.FileName );

    _versionService.Value.CheckByPathIfFileExists( oldFilePath );

    File.Move( oldFilePath, newFilePath );

    file.FileName = data.FileName;
    file.FilePath = newFilePath;
    file.UpdatedAt = DateTime.UtcNow;

    _dbContext.Files.Update( file );
    _dbContext.SaveChanges();

    return FileToDto( file );
  }

  public FileDto Remove( int id )
  {
    MediaFile? file = CheckIfFileExists( id );

    file.DeletedAt = DateTime.UtcNow;

    _dbContext.Files.Remove( file );
    _dbContext.SaveChanges();

    return FileToDto( file );
  }


  public MediaFile CheckIfFileExists( int fileId )
  {
    MediaFile? file = _dbContext.Files.Find( fileId );

    if ( file == null )
    {
      LoggingHelper.LogFailure( _logger, "File not found", new { Id = fileId } );

      throw new ServerException( $"File with ID='{fileId}' not found", ExceptionStatusCode.FileNotFound );
    }
    return file;
  }

  public void CheckIfFileIsNull( MediaFile? file )
  {
    if ( file == null )
    {
      LoggingHelper.LogFailure( _logger, "File not found" );

      throw new ServerException( $"File not found", ExceptionStatusCode.FileNotFound );
    }
  }

  private string SaveFile( UploadFileDto data, string fileName, string fileType )
  {
    if ( data == null || data.FileData == null || string.IsNullOrEmpty( fileName ) )
    {
      LoggingHelper.LogFailure( _logger, "Invalid file upload data" );

      throw new ServerException( "Invalid file upload data", ExceptionStatusCode.BadRequest );
    }

    if ( !ValidationHelpers.BeAValidFileType( fileType ) )
    {
      LoggingHelper.LogFailure( _logger, "Invalid file type" );

      throw new ServerException( "Invalid file type", ExceptionStatusCode.UnsupportedMediaType );
    }

    string filePath = PathHelper.GetFilePath( data.UserId, fileName );

    try
    {
      File.WriteAllBytes( filePath, data.FileData );
    }
    catch ( IOException )
    {
      LoggingHelper.LogFailure( _logger, "Failed to save file due to storage issues." );

      throw new ServerException( "Failed to save file due to storage issues.", ExceptionStatusCode.InsufficientStorage );
    }
    catch ( UnauthorizedAccessException )
    {
      LoggingHelper.LogFailure( _logger, "Insufficient permissions to save file." );

      throw new ServerException( "Insufficient permissions to save file.", ExceptionStatusCode.Forbidden );
    }

    return filePath;
  }

  private FileDto FileToDto( MediaFile file )
  {
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
      DeletedAt = file.DeletedAt
    };
  }

  private void CheckFileDuplicate( string fileName, int userId )
  {
    MediaFile? file = _dbContext.Files.FirstOrDefault(
     f => f.FileName.Equals( fileName )
       && f.UserId.Equals( userId )
   );

    if ( file != null )
    {
      throw new ServerException( $"File with Name {file.FileName} already exists", ExceptionStatusCode.FileDuplicate );
    }
  }
}
