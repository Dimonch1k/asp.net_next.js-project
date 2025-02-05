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

  public ShareFileDto ShareFile( ShareFileDto data )
  {
    SharedFile sharedFile = new SharedFile
    {
      Id = data.Id,
      FileId = data.FileId,
      OwnerId = data.OwnerId,
      SharedWithId = data.SharedWithId,
      Permission = (AccessType) Enum.Parse( typeof( AccessType ), data.Permission.ToLower() )
    };

    _dbContext.SharedFiles.Add( sharedFile );
    _dbContext.SaveChanges();

    return SharedFileToDto( sharedFile );
  }

  public ShareFileDto DeleteSharedFile( int id )
  {
    SharedFile? sharedFile = GetSharedFileIfExists( id );

    _dbContext.SharedFiles.Remove( sharedFile );
    _dbContext.SaveChanges();

    return SharedFileToDto( sharedFile );
  }


  public SharedFile GetSharedFileIfExists( int sharedFileId )
  {
    SharedFile? sharedFile = _dbContext.SharedFiles.Find( sharedFileId );

    if ( sharedFile == null )
    {
      _logger.LogError( "Shared file not found" );

      throw new ServerException( $"Shared file with ID='{sharedFileId}' not found", ExceptionStatusCode.SharedFileNotFound );
    }

    return sharedFile;
  }

  public void EnsureSharedFileIsNotNull( SharedFile? sharedFile )
  {
    if ( sharedFile == null )
    {
      _logger.LogError( "Shared file not found" );

      throw new ServerException( $"Shared file not found", ExceptionStatusCode.SharedFileNotFound );
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
