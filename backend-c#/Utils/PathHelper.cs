using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend_c_.Utils;

public class PathHelper
{
  //private static readonly string BaseDirectory = Environment.GetEnvironmentVariable( "FILE_BASE_DIRECTORY" )
  //  ?? throw new InvalidOperationException( "Base directory is not configured" );
  public static readonly string BaseDirectory = @"C:\Users\dmytro\Desktop\uploads";

  public static string GetVersionPath( int userId, int fileId, string name )
  {
    string directoryPath = Path.Combine( BaseDirectory, userId.ToString(), "versions", fileId.ToString() );

    CreateDirectory( directoryPath );

    return Path.Combine( directoryPath, name );
  }

  public static string GetFilePath( int userId, string name )
  {
    string directoryPath = Path.Combine( BaseDirectory, userId.ToString() );

    CreateDirectory( directoryPath );

    return Path.Combine( directoryPath, name );
  }

  public static string UpdatePath( string? oldPath, string newFileName )
  {
    string? directory = Path.GetDirectoryName( oldPath );

    if ( directory == null )
    {
      throw new InvalidOperationException( "Invalid path" );
    }
    string? extension = Path.GetExtension( oldPath );

    return Path.Combine( directory, $"{Path.GetFileNameWithoutExtension( newFileName )}{extension}" );
  }


  private static void CreateDirectory( string path )
  {
    if ( !Directory.Exists( path ) )
    {
      Directory.CreateDirectory( path );
    }
  }
}
