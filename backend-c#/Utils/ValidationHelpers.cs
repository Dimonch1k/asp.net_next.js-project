using System.Linq;
using backend_c_.Enums;
using backend_c_.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend_c_.Utils;

public static class ValidationHelpers
{
  private static readonly HashSet<string> AllowedFileTypes = new()
  {
      "text/plain", // .txt
      "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // .docx
      "application/vnd.openxmlformats-officedocument.presentationml.presentation", // .pptx
      "application/pdf", // .pdf
      "image/jpeg", // .jpg/.jpeg
      "image/png", // .png
      "audio/mpeg", // .mp3
      "video/mp4" // .mp4
  };

  public static bool BeAValidFileType( string fileType )
  {
    return AllowedFileTypes.Contains( fileType );
  }

  public static void EnsureFileHasValidFileType( string fileType )
  {
    if ( !AllowedFileTypes.Contains( fileType ) )
    {
      throw new ServerException( "Invalid file type. Only such extensions are supported: .txt, .docx, .pptx, .pdf, .jpg, .jpeg, .png, .mp3, .mp4", ExceptionStatusCode.UnsupportedMediaType );
    }
  }

  public static bool BeAValidTimeZone( string timeZoneId )
  {
    return TimeZoneInfo.GetSystemTimeZones().Any( tz => tz.Id == timeZoneId );
  }

  public static bool BeAValidAccessType( string accessType )
  {
    return Enum.IsDefined( typeof( AccessType ), accessType.ToLower() );
  }

  public static AccessType EnsureAccessTypeExists( string accessType )
  {
    //    Enum.TryParse( enumType, value, ignoreCase, result )
    if ( !Enum.TryParse( typeof( AccessType ), accessType, true, out object? result )
      || result is null )
    {
      throw new ServerException( "Invalid access type provided.", ExceptionStatusCode.NoAccessProvided );
    }

    return (AccessType) result;
  }
}
