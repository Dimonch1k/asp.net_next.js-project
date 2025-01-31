using System.Xml.Linq;
using backend_c_.Enums;
using backend_c_.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend_c_.Utils;

public class PathHelper
{
  //private static readonly string BaseDirectory = Environment.GetEnvironmentVariable( "FILE_BASE_DIRECTORY" )
  //  ?? throw new ServerException( "Base directory is not configured", ExceptionStatusCode.InternalServerError );
  public static readonly string BaseDirectory = @"C:\Users\dmytro\Desktop\uploads";

  public PathHelper( )
  {
    if ( string.IsNullOrEmpty( BaseDirectory ) || !Directory.Exists( BaseDirectory ) )
    {
      throw new ServerException( "Internal server error.", Enums.ExceptionStatusCode.InternalServerError );
    }
  }

  public static string GetVersionPath( int userId, int fileId, string name )
  {
    string directoryPath = Path.Combine( BaseDirectory, userId.ToString(), "versions", fileId.ToString() );

    EnsureDirectoryExists( directoryPath );

    return Path.Combine( directoryPath, name );
  }

  public static string GetFilePath( int userId, string name )
  {
    string directoryPath = Path.Combine( BaseDirectory, userId.ToString() );

    EnsureDirectoryExists( directoryPath );

    return Path.Combine( directoryPath, name );
  }

  public static string UpdatePath( string? oldPath, string newFileName )
  {
    if ( string.IsNullOrEmpty( oldPath ) || !Path.IsPathFullyQualified( oldPath ) )
    {
      throw new ServerException( "Invalid or empty path provided.", ExceptionStatusCode.BadRequest );
    }

    string? directory = Path.GetDirectoryName( oldPath );

    if ( directory == null )
    {
      throw new ServerException( "Invalid path: Directory is null.", ExceptionStatusCode.DirectoryNotFound );
    }

    string? extension = Path.GetExtension( oldPath );
    return Path.Combine( directory, $"{Path.GetFileNameWithoutExtension( newFileName )}{extension}" );
  }


  private static void EnsureDirectoryExists( string path )
  {
    if ( !Directory.Exists( path ) )
    {
      Directory.CreateDirectory( path );
    }
  }
}
