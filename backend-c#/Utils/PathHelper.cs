using System.Xml.Linq;
using backend_c_.Enums;
using backend_c_.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend_c_.Utils;

public class PathHelper
{
  //private static readonly string PATH_BASE_DIR = Environment.GetEnvironmentVariable( "FILE_BASE_DIRECTORY_LINUX" );
  private static readonly string? PATH_BASE_DIR = Environment.GetEnvironmentVariable( "PATH_BASE_DIRECTORY_WINDOWS" );

  public static readonly string _baseDirectory = PATH_BASE_DIR;
  public static readonly string _tempFolder = Path.Combine( PATH_BASE_DIR, "tempUploads" );

  public PathHelper( )
  {
    if ( string.IsNullOrEmpty( _baseDirectory ) || !Directory.Exists( _baseDirectory ) )
    {
      throw new ServerException( "Internal server error.", Enums.ExceptionStatusCode.InternalServerError );
    }
  }

  public static string GetVersionPath( int userId, int fileId, string name )
  {
    string directoryPath = Path.Combine( _baseDirectory, userId.ToString(), "versions", fileId.ToString() );

    EnsureDirectoryExists( directoryPath );

    return Path.Combine( directoryPath, name );
  }

  public static string GetFilePath( int userId, string name )
  {
    string directoryPath = Path.Combine( _baseDirectory, userId.ToString() );

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

  public static void EnsureDirectoryExists( string path )
  {
    if ( !Directory.Exists( path ) )
    {
      Directory.CreateDirectory( path );
    }
  }

  public static void EnsureFileExistsAtPath( string path )
  {
    if ( !File.Exists( path ) )
    {
      throw new ServerException( $"File not found at {path}", ExceptionStatusCode.DirectoryNotFound );
    }
  }
}
