using backend_c_.DTO.SharedFile;
using backend_c_.DTO.File;
using backend_c_.Entity;
using backend_c_.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using backend_c_.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;
using backend_c_.Exceptions;
using backend_c_.Utilities;
using backend_c_.DTO.Access;
using System.Diagnostics.Eventing.Reader;

namespace backend_c_.Service.Impl;

public class SharedFileService : ISharedFileService
{
  private readonly AppDbContext _dbContext;
  private readonly IConfiguration _configuration;
  private readonly ILogger<SharedFileService> _logger;

  public SharedFileService( AppDbContext dbContext, IConfiguration configuration, ILogger<SharedFileService> logger )
  {
    _dbContext = dbContext;
    _configuration = configuration;
    _logger = logger;
  }

  public IEnumerable<ShareFileDto> GetAllSharedFiles( )
  {
    return _dbContext.SharedFiles
      .Select( SharedFileToDto )
      .ToArray();
  }

  public IEnumerable<ShareFileDto> GetMySharedFiles( int ownerId )
  {
    return _dbContext.SharedFiles
      .Where( sh => sh.OwnerId == ownerId )
      .Select( SharedFileToDto )
      .ToArray();
  }

  public ShareFileDto ShareFile( ShareFileDto data )
  {
    // Verifications for files and users correctness was placed into controller
    // to prevent services' referencing to each other

    EnsureSharedFileUnique( data.OwnerId, data.SharedWithId, data.FileId );

    AccessType validatedPermission = ValidationHelpers.EnsureAccessTypeExists( data.Permission );

    SharedFile sharedFile = new SharedFile
    {
      Id = data.Id,
      FileId = data.FileId,
      OwnerId = data.OwnerId,
      SharedWithId = data.SharedWithId,
      Permission = validatedPermission
    };

    _dbContext.SharedFiles.Add( sharedFile );
    _dbContext.SaveChanges();

    return SharedFileToDto( sharedFile );
  }

  public async Task<ShareFileDto> DeleteSharedFile( int? id )
  {
    SharedFile? sharedFile = await GetSharedFileIfExists( id );

    _dbContext.SharedFiles.Remove( sharedFile );
    _dbContext.SaveChanges();

    return SharedFileToDto( sharedFile );
  }

  public async Task<SharedFile> GetSharedFileIfExists( int? sharedFileId )
  {
    SharedFile? sharedFile = await _dbContext.SharedFiles.FindAsync( sharedFileId );

    if ( sharedFile == null )
    {
      _logger.LogError( "Shared file not found" );

      throw new ServerException( $"Shared file with ID='{sharedFileId}' not found", ExceptionStatusCode.SharedFileNotFound );
    }

    return sharedFile;
  }

  public void EnsureSharedFileUnique( int ownerId, int sharedWithId, int fileId )
  {
    SharedFile? sharedFile = _dbContext.SharedFiles.FirstOrDefault(
      sh => sh.OwnerId == ownerId
         && sh.SharedWithId == sharedWithId
         && sh.FileId == fileId );

    if ( sharedFile != null )
    {
      _logger.LogError( $"You have already shared the file with ID: '{fileId}' to user with ID: '{sharedWithId}'" );

      throw new ServerException( $"You have already shared the file with ID: '{fileId}' to user with ID: '{sharedWithId}'", ExceptionStatusCode.SharedFileDuplicate );
    }
  }

  public ShareFileDto SharedFileToDto( SharedFile sharedFile )
  {
    return new ShareFileDto
    {
      Id = sharedFile.Id,
      FileId = sharedFile.FileId,
      OwnerId = sharedFile.OwnerId,
      SharedWithId = sharedFile.SharedWithId,
      Permission = sharedFile.Permission.ToString().ToLower(),
    };
  }
}
