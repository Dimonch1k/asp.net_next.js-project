using System.Linq;
using backend_c_.Enums;
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
      "image/gif", // .gif
      "audio/mpeg", // .mp3
      "video/mp4" // .mp4
  };

  public static bool BeAValidFileType( string fileType )
  {
    return AllowedFileTypes.Contains( fileType );
  }

  public static bool BeAValidTimeZone( string timeZoneId )
  {
    return TimeZoneInfo.GetSystemTimeZones().Any( tz => tz.Id == timeZoneId );
  }

  public static bool BeAValidAccessType( string accessType )
  {
    return Enum.IsDefined( typeof( AccessType ), accessType.ToLower() );
  }
}
