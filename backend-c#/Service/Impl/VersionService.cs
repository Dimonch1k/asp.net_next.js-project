using backend_c_.DTO.Version;
using backend_c_.Entity;
using backend_c_.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace backend_c_.Service.Impl;

public class VersionService : IVersionService
{
  private readonly AppDbContext _dbContext;

  public VersionService( AppDbContext dbContext )
  {
    _dbContext = dbContext;
  }

  public IEnumerable<FileVersionDto> FindAll( )
  {
    return _dbContext.FileVersions
      .Select( VersionToDto )
      .ToList();
  }

  public IEnumerable<FileVersionDto> FindByFileId( int fileId )
  {
    if ( !_dbContext.Files.Any( file => file.Id == fileId ) )
    {
      throw new InvalidOperationException( $"File with ID {fileId} does not exist." );
    }

    return _dbContext.FileVersions
      .Where( fileVersion => fileVersion.FileId == fileId )
      .Select( VersionToDto )
      .ToList();
  }

  public FileVersionDto FindOne( int id )
  {
    FileVersion? fileVersion = _dbContext.FileVersions.Find( id );

    if ( fileVersion == null )
    {
      throw new InvalidOperationException( $"File version with ID {id} does not exist." );
    }

    return VersionToDto( fileVersion );
  }

  public FileVersionDto Create( CreateFileVersionDto data )
  {
    MediaFile? file = _dbContext.Files.FirstOrDefault( f => f.Id == data.FileId );

    if ( file == null )
    {
      throw new InvalidOperationException( $"File with ID {data.FileId} does not exist." );
    }

    string currentFilePath = file.FilePath;

    if ( !File.Exists( currentFilePath ) )
    {
      throw new Exception( $"Original file not found at {currentFilePath}." );
    }

    User? user = _dbContext.Users.FirstOrDefault( user => user.Id == file.UserId );

    if ( user == null )
    {
      throw new InvalidOperationException( $"User with ID {file.UserId} doesn't exist" );
    }

    string destinationPath = PathHelper.GetVersionPath( user.Id, data.FileId, data.VersionName );

    File.Copy( currentFilePath, destinationPath, overwrite: true );

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
    FileVersion? fileVersion = _dbContext.FileVersions.Find( versionId );

    if ( fileVersion == null )
    {
      throw new InvalidOperationException( $"File version with ID {versionId} does not exist." );
    }

    MediaFile? file = _dbContext.Files.FirstOrDefault( f => f.Id == fileVersion.FileId );

    if ( file == null )
    {
      throw new InvalidOperationException( $"File with ID {fileVersion.FileId} does not exist." );
    }

    if ( !File.Exists( fileVersion.VersionPath ) )
    {
      throw new Exception( $"Version file not found at {fileVersion.VersionPath}." );
    }

    File.Copy( fileVersion.VersionPath, file.FilePath, overwrite: true );
    file.UpdatedAt = DateTime.UtcNow;

    _dbContext.Files.Update( file );
    _dbContext.SaveChanges();

    return VersionToDto( fileVersion );
  }

  public FileVersionDto Update( int id, UpdateFileVersionDto data )
  {
    FileVersion? fileVersion = _dbContext.FileVersions.Find( id );

    if ( fileVersion == null )
    {
      throw new InvalidOperationException( $"File version with ID {id} does not exist." );
    }

    string oldFileVersionPath = fileVersion.VersionPath;
    string newFileVersionPath = PathHelper.UpdatePath( fileVersion.VersionPath, data.VersionName );

    if ( !File.Exists( oldFileVersionPath ) )
    {
      throw new Exception( $"File version not found" );
    }
    File.Move( oldFileVersionPath, newFileVersionPath );

    fileVersion.VersionName = data.VersionName;
    fileVersion.VersionPath = newFileVersionPath;

    _dbContext.FileVersions.Update( fileVersion );
    _dbContext.SaveChanges();

    return VersionToDto( fileVersion );
  }

  public FileVersionDto Remove( int id )
  {
    FileVersion? fileVersion = _dbContext.FileVersions.Find( id );

    if ( fileVersion == null )
    {
      throw new InvalidOperationException( $"File version with ID {id} does not exist." );
    }

    _dbContext.FileVersions.Remove( fileVersion );
    _dbContext.SaveChanges();

    return VersionToDto( fileVersion );
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
