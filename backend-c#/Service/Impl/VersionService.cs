using backend_c_.DTO.Version;
using backend_c_.Entity;
using backend_c_.Enums;
using backend_c_.Exceptions;
using backend_c_.Utilities;
using backend_c_.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace backend_c_.Service.Impl;

public class VersionService : IVersionService
{
  private readonly AppDbContext _dbContext;
  private readonly IUserService _userService;
  private readonly IFileService _fileService;
  private readonly ILogger<VersionService> _logger;

  public VersionService( AppDbContext dbContext, IUserService userService, IFileService fileService, ILogger<VersionService> logger )
  {
    _dbContext = dbContext;
    _userService = userService;
    _fileService = fileService;
    _logger = logger;
  }

  public IEnumerable<FileVersionDto> FindAll( )
  {
    return _dbContext.FileVersions
      .Select( VersionToDto )
      .ToList();
  }

  public IEnumerable<FileVersionDto> FindByFileId( int fileId )
  {
    _fileService.CheckIfFileExists( fileId );

    return _dbContext.FileVersions
      .Where( fileVersion => fileVersion.FileId == fileId )
      .Select( VersionToDto )
      .ToList();
  }

  public FileVersionDto FindOne( int id )
  {
    FileVersion? fileVersion = CheckIfFileVersionExists( id );

    return VersionToDto( fileVersion );
  }

  public FileVersionDto Create( CreateFileVersionDto data )
  {
    MediaFile? file = _fileService.CheckIfFileExists( data.FileId );

    CheckByPathIfFileExists( file.FilePath );

    User? user = _dbContext.Users.FirstOrDefault( user => user.Id == file.UserId );

    _userService.CheckIfUserIsNull( user );

    string destinationPath = PathHelper.GetVersionPath( user.Id, data.FileId, data.VersionName );

    File.Copy( file.FilePath, destinationPath, overwrite: true );

    FileVersion newFileVersion = new FileVersion
    {
      FileId = data.FileId,
      VersionName = data.VersionName,
      VersionPath = destinationPath,
      CreatedAt = DateTime.UtcNow
    };

    _dbContext.FileVersions.Add( newFileVersion );
    _dbContext.SaveChanges();

    return VersionToDto( newFileVersion );
  }

  public FileVersionDto RestoreVersion( int versionId )
  {
    FileVersion? fileVersion = CheckIfFileVersionExists( versionId );

    MediaFile? file = _dbContext.Files.FirstOrDefault( f => f.Id == fileVersion.FileId );

    _fileService.CheckIfFileIsNull( file );

    CheckByPathIfFileExists( fileVersion.VersionPath );

    File.Copy( fileVersion.VersionPath, file.FilePath, overwrite: true );
    file.UpdatedAt = DateTime.UtcNow;

    _dbContext.Files.Update( file );
    _dbContext.SaveChanges();

    return VersionToDto( fileVersion );
  }

  public FileVersionDto Update( int id, UpdateFileVersionDto data )
  {
    FileVersion? fileVersion = CheckIfFileVersionExists( id );

    string oldFileVersionPath = fileVersion.VersionPath;
    string newFileVersionPath = PathHelper.UpdatePath( fileVersion.VersionPath, data.VersionName );

    CheckByPathIfFileExists( oldFileVersionPath );

    File.Move( oldFileVersionPath, newFileVersionPath );

    fileVersion.VersionName = data.VersionName;
    fileVersion.VersionPath = newFileVersionPath;

    _dbContext.FileVersions.Update( fileVersion );
    _dbContext.SaveChanges();

    return VersionToDto( fileVersion );
  }

  public FileVersionDto Remove( int id )
  {
    FileVersion? fileVersion = CheckIfFileVersionExists( id );

    _dbContext.FileVersions.Remove( fileVersion );
    _dbContext.SaveChanges();

    return VersionToDto( fileVersion );
  }


  public FileVersion CheckIfFileVersionExists( int fileVersionId )
  {
    FileVersion? fileVersion = _dbContext.FileVersions.Find( fileVersionId );

    if ( fileVersion == null )
    {
      LoggingHelper.LogFailure( _logger, "File version not found", new { Id = fileVersionId } );

      throw new ServerException( $"File version with ID='{fileVersionId}' not found", ExceptionStatusCode.FileVersionNotFound );
    }

    return fileVersion;
  }

  private FileVersionDto VersionToDto( FileVersion version )
  {
    return new FileVersionDto
    {
      Id = version.Id,
      FileId = version.FileId,
      VersionName = version.VersionName,
      VersionPath = version.VersionPath
    };
  }

  private void CheckByPathIfFileExists( string path )
  {
    if ( !File.Exists( path ) )
    {
      LoggingHelper.LogFailure( _logger, $"File not found at {path}" );

      throw new ServerException( $"File not found at {path}", ExceptionStatusCode.DirectoryNotFound );
    }
  }
}
