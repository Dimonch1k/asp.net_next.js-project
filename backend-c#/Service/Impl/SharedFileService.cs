using backend_c_.DTO.SharedFile;
using backend_c_.DTO.File;
using backend_c_.Entity;
using backend_c_.Helpers;
using backend_c_.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using backend_c_.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend_c_.Service.Impl;

public class SharedFileService : ISharedFileService
{
  private readonly AppDbContext _dbContext;
  private readonly IConfiguration _configuration;

  public SharedFileService( AppDbContext dbContext, IConfiguration configuration )
  {
    _dbContext = dbContext;
    _configuration = configuration;
  }

  public ShareFileDto Share( ShareFileDto data )
  {
    MediaFile? file = _dbContext.Files.FirstOrDefault( f => f.Id == data.FileId );

    if ( file == null )
    {
      throw new Exception( "File not found." );
    }

    SharedFile sharedFile = new SharedFile
    {
      Id = data.Id,
      FileId = data.FileId,
      OwnerId = data.OwnerId,
      SharedWithId = data.SharedWithId,
      Permission = (AccessType) Enum.Parse(typeof(AccessType), data.Permission.ToLower() )
    };

    _dbContext.SharedFiles.Add( sharedFile );
    _dbContext.SaveChanges();

    return SharedFileToDto( sharedFile );
  }

  public ShareFileDto Remove( int id )
  {
    SharedFile? sharedFile = _dbContext.SharedFiles.FirstOrDefault( sf => sf.Id == id );

    if ( sharedFile == null )
    {
      throw new Exception( "No shared file found" );
    }

    _dbContext.SharedFiles.Remove( sharedFile );
    _dbContext.SaveChanges();

    return SharedFileToDto( sharedFile );
  }

  public static ShareFileDto SharedFileToDto( SharedFile sharedFile )
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
