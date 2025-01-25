using backend_c_.DTO.File;
using backend_c_.Utils;

namespace backend_c_.Helpers;

public static class FileUploadHelper
{
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

    string filePath = PathHelper.GetFilePath( data.UserId, fileName );

    File.WriteAllBytes( filePath, data.FileData );

    return filePath;
  }
}
