using backend_c_.DTO.File;
using backend_c_.DTO.SharedFile;
using backend_c_.DTO.User;
using backend_c_.Entity;
using backend_c_.Enums;
using backend_c_.Exceptions;
using backend_c_.Utilities;
using backend_c_.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend_c_.Service.Impl;

public class MediaFileService : IFileService
{
  private readonly AppDbContext _dbContext;
  private readonly IUserService _userService;
  private readonly ISharedFileService _sharedFileService;
  private readonly IVersionService _versionService;
  private readonly IConfiguration _configuration;
  private readonly ILogger<MediaFileService> _logger;

  public MediaFileService( AppDbContext dbContext, IUserService userService, ISharedFileService sharedFileService, IVersionService versionService, IConfiguration configuration, ILogger<MediaFileService> logger )
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
    _userService.CheckIfUserExists( userId );

    return _dbContext.SharedFiles
      .Where( sf => sf.OwnerId == userId )
      .Select( sf => _sharedFileService.SharedFileToDto( sf ) )
      .ToList();
  }

  public IEnumerable<ShareFileDto> GetFilesSharedToMe( int userId )
  {
    _userService.CheckIfUserExists( userId );

    return _dbContext.SharedFiles
      .Where( sf => sf.SharedWithId == userId )
      .Select( sf => SharedFileService.SharedFileToDto( sf ) )
      .ToList();
  }

  public IEnumerable<FileDto> GetUserFiles( int userId )
  {
    _userService.CheckIfUserExists( userId );

    return _dbContext.Files
      .Where( file => file.UserId == userId )
      .Select( file => FileToDto( file ) )
      .ToList();
  }

  public FileDto Upload( UploadFileDto data, IFormFile uploadedFile )
  {
    _userService.CheckIfUserExists( data.UserId );

    MediaFile? file = _dbContext.Files.FirstOrDefault(
      f => f.FileName.Equals( uploadedFile.FileName )
        && f.UserId.Equals( data.UserId )
    );

    if ( file != null )
    {
      throw new ServerException( $"File with Name {file.FileName} already exists", ExceptionStatusCode.FileDuplicate );
    }

    string filePath = SaveFile( data, uploadedFile.FileName, uploadedFile.ContentType );

    MediaFile newFile = new MediaFile
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
    _dbContext.SaveChanges();

    return FileToDto( newFile );
  }

  public FileDto Update( int id, UpdateFileDto data )
  {
    MediaFile? file = CheckIfFileExists( id );

    string oldFilePath = file.FilePath;
    string newFilePath = PathHelper.UpdatePath( file.FilePath, data.FileName );

    _versionService.CheckByPathIfFileExists( oldFilePath );

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
}
