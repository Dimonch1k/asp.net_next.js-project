using backend_c_.DTO.File;
using backend_c_.DTO.SharedFile;
using backend_c_.DTO.User;
using backend_c_.Entity;
using backend_c_.Helpers;
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
  private readonly IConfiguration _configuration;

  public MediaFileService( AppDbContext dbContext, IConfiguration configuration )
  {
    _dbContext = dbContext;
    _configuration = configuration;
  }

  public IEnumerable<ShareFileDto> GetFilesSharedByMe( int userId )
  {
    CheckIfUserExists( userId );

    return _dbContext.SharedFiles
      .Where( sf => sf.OwnerId == userId )
      .Select( sf => SharedFileService.SharedFileToDto( sf ) )
      .ToList();
  }

  public IEnumerable<ShareFileDto> GetFilesSharedToMe( int userId )
  {
    CheckIfUserExists( userId );

    return _dbContext.SharedFiles
      .Where( sf => sf.SharedWithId == userId )
      .Select( sf => SharedFileService.SharedFileToDto( sf ) )
      .ToList();
  }

  public IEnumerable<FileDto> GetUserFiles( int userId )
  {
    CheckIfUserExists( userId );

    return _dbContext.Files
      .Where( file => file.UserId == userId )
      .Select( file => FileToDto( file ) )
      .ToList();
  }

  public FileDto Upload( UploadFileDto data, IFormFile uploadedFile )
  {
    CheckIfUserExists( data.UserId );

    MediaFile? file = _dbContext.Files.FirstOrDefault( f => f.FileName == uploadedFile.FileName && f.UserId == data.UserId );

    if ( file != null )
    {
      throw new Exception( $"File with Name {file.FileName} already exists" );
    }

    string filePath = FileUploadHelper.SaveFile( data, uploadedFile.FileName, uploadedFile.ContentType );

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
    MediaFile? file = _dbContext.Files.FirstOrDefault( f => f.Id == id );

    if ( file == null )
    {
      throw new Exception( $"File with ID {id} not found" );
    }

    string oldFilePath = file.FilePath;
    string newFilePath = PathHelper.UpdatePath( file.FilePath, data.FileName );

    if ( !File.Exists( oldFilePath ) )
    {
      throw new Exception( $"File with path '{oldFilePath}' not found" );
    }
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
    MediaFile? file = _dbContext.Files.FirstOrDefault( f => f.Id == id );

    if ( file == null )
    {
      throw new Exception( "File not found" );
    }
    file.DeletedAt = DateTime.UtcNow;

    _dbContext.Files.Remove( file );
    _dbContext.SaveChanges();

    return FileToDto( file );
  }


  private void CheckIfUserExists(int id)
  {
    User? user = _dbContext.Users.FirstOrDefault( u => u.Id == id );

    if ( user == null )
    {
      throw new Exception( $"User with ID {id} not found." );
    }
  }
  
  private static FileDto FileToDto( MediaFile file )
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
