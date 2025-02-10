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
using System.Runtime.CompilerServices;

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
    // Verification if file exists is in controller to prevent from services' refering loop
    return _dbContext.FileVersions
      .Where( fileVersion => fileVersion.FileId == fileId )
      .Select( VersionToDto )
      .ToList();
  }

  public async Task<FileVersionDto> GetVersionById( int id )
  {
    FileVersion? fileVersion = await GetFileVersionIfExists( id );

    return VersionToDto( fileVersion );
  }

  public async Task<FileVersionDto> CreateVersion( CreateFileVersionDto data, MediaFile file )
  {
    PathHelper.EnsureFileExistsAtPath( file.FilePath );

    EnsureFileVersionIsUnique( data.VersionName, data.FileId );

    User? user = await _userService.Value.GetUserIfExists( file.UserId );

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
    PathHelper.EnsureFileExistsAtPath( fileVersion.VersionPath );

    File.Copy( fileVersion.VersionPath, file.FilePath, overwrite: true );
    file.UpdatedAt = DateTime.UtcNow;

    _dbContext.Files.Update( file );
    _dbContext.SaveChanges();

    return VersionToDto( fileVersion );
  }

  public async Task<FileVersionDto> UpdateVersion( int id, UpdateFileVersionDto data )
  {
    FileVersion? fileVersion = await GetFileVersionIfExists( id );

    EnsureFileVersionIsUnique( data.VersionName, fileVersion.FileId );

    string oldFileVersionPath = fileVersion.VersionPath;
    string newFileVersionPath = PathHelper.UpdatePath( fileVersion.VersionPath, data.VersionName );

    PathHelper.EnsureFileExistsAtPath( oldFileVersionPath );

    // File path name change, not moving file version to new path
    File.Move( oldFileVersionPath, newFileVersionPath );

    fileVersion.VersionName = data.VersionName;
    fileVersion.VersionPath = newFileVersionPath;

    _dbContext.FileVersions.Update( fileVersion );
    _dbContext.SaveChanges();

    return VersionToDto( fileVersion );
  }

  public async Task<FileVersionDto> DeleteVersion( int id )
  {
    FileVersion? fileVersion = await GetFileVersionIfExists( id );

    _dbContext.FileVersions.Remove( fileVersion );
    _dbContext.SaveChanges();

    return VersionToDto( fileVersion );
  }


  public async Task<FileVersion> GetFileVersionIfExists( int fileVersionId )
  {
    FileVersion? fileVersion = await _dbContext.FileVersions.FindAsync( fileVersionId );

    if ( fileVersion == null )
    {
      _logger.LogError( "File version not found" );

      throw new ServerException( $"File version with ID='{fileVersionId}' not found", ExceptionStatusCode.FileVersionNotFound );
    }

    return fileVersion;
  }

  private void EnsureFileVersionIsUnique( string versionName, int fileId )
  {
    FileVersion? version = _dbContext.FileVersions.FirstOrDefault(
      v => v.VersionName == versionName
        && v.FileId == fileId
    );

    if ( version != null )
    {
      _logger.LogError( $"The file version with Name: '{versionName}' already exists. Choose another name" );

      throw new ServerException( $"The file version with Name: '{versionName}' already exists. Choose another name", ExceptionStatusCode.FileVersionDuplicate );
    }
  }

  private static FileVersionDto VersionToDto( FileVersion version )
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
