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
  private readonly Lazy<IUserService> _userService;
  private readonly ILogger<VersionService> _logger;

  public VersionService( AppDbContext dbContext, Lazy<IUserService> userService, ILogger<VersionService> logger )
  {
    _dbContext = dbContext;
    _userService = userService;
    _logger = logger;
  }

  public IEnumerable<FileVersionDto> GetAllVersions( )
  {
    return _dbContext.FileVersions
      .Select( VersionToDto )
      .ToList();
  }

  public IEnumerable<FileVersionDto> GetVersionsByFileId( int fileId )
  {
    return _dbContext.FileVersions
      .Where( fileVersion => fileVersion.FileId == fileId )
      .Select( VersionToDto )
      .ToList();
  }

  public FileVersionDto GetVersionById( int id )
  {
    FileVersion? fileVersion = GetFileVersionIfExists( id );

    return VersionToDto( fileVersion );
  }

  public FileVersionDto CreateVersion( CreateFileVersionDto data, MediaFile file )
  {
    EnsureFileExistsAtPath( file.FilePath );

    User? user = _userService.Value.GetUserIfExists( file.UserId );

    _userService.Value.EnsureUserIsNotNull( user );

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

  public FileVersionDto RestoreFileVersion( int versionId, MediaFile file, FileVersion fileVersion )
  {
    EnsureFileExistsAtPath( fileVersion.VersionPath );

    File.Copy( fileVersion.VersionPath, file.FilePath, overwrite: true );
    file.UpdatedAt = DateTime.UtcNow;

    _dbContext.Files.Update( file );
    _dbContext.SaveChanges();

    return VersionToDto( fileVersion );
  }

  public FileVersionDto UpdateVersion( int id, UpdateFileVersionDto data )
  {
    FileVersion? fileVersion = GetFileVersionIfExists( id );

    string oldFileVersionPath = fileVersion.VersionPath;
    string newFileVersionPath = PathHelper.UpdatePath( fileVersion.VersionPath, data.VersionName );

    EnsureFileExistsAtPath( oldFileVersionPath );

    File.Move( oldFileVersionPath, newFileVersionPath );

    fileVersion.VersionName = data.VersionName;
    fileVersion.VersionPath = newFileVersionPath;

    _dbContext.FileVersions.Update( fileVersion );
    _dbContext.SaveChanges();

    return VersionToDto( fileVersion );
  }

  public FileVersionDto DeleteVersion( int id )
  {
    FileVersion? fileVersion = GetFileVersionIfExists( id );

    _dbContext.FileVersions.Remove( fileVersion );
    _dbContext.SaveChanges();

    return VersionToDto( fileVersion );
  }


  public FileVersion GetFileVersionIfExists( int fileVersionId )
  {
    FileVersion? fileVersion = _dbContext.FileVersions.Find( fileVersionId );

    if ( fileVersion == null )
    {
      _logger.LogError( "File version not found" );

      throw new ServerException( $"File version with ID='{fileVersionId}' not found", ExceptionStatusCode.FileVersionNotFound );
    }

    return fileVersion;
  }

  public void EnsureFileExistsAtPath( string path )
  {
    if ( !File.Exists( path ) )
    {
      _logger.LogError( $"File not found at {path}" );

      throw new ServerException( $"File not found at {path}", ExceptionStatusCode.DirectoryNotFound );
    }
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
}
