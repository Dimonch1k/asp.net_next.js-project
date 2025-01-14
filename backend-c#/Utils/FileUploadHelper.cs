using backend_c_.DTO.File;
using backend_c_.Utils;

namespace backend_c_.Helpers;

public static class FileUploadHelper
{
  //private static readonly string BaseDirectory = Environment.GetEnvironmentVariable( "FILE_BASE_DIRECTORY" )
  //  ?? throw new InvalidOperationException( "Base directory is not configured" );
  private static readonly string BaseDirectory = @"C:\Users\dmytro\Desktop\uploads";

  public static string SaveFile( UploadFileDto data, string fileName, string fileType )
  {
    if ( data == null || data.FileData == null || string.IsNullOrEmpty( fileName ) )
    {
      throw new ArgumentException( "Invalid file upload data" );
    }

    if ( !ValidationHelpers.BeAValidFileType( fileType ) )
    {
      throw new InvalidOperationException( "Invalid file type" );
    }

    string userDirectory = Path.Combine( BaseDirectory, data.UserId.ToString() );

    if ( !Directory.Exists( userDirectory ) )
    {
      Directory.CreateDirectory( userDirectory );
    }

    string filePath = Path.Combine( userDirectory, fileName );

    File.WriteAllBytes( filePath, data.FileData );

    return filePath;
  }
}
