namespace backend_c_.Enums;

public enum ExceptionStatusCode
{
  Unauthorized = 401,
  UserNotFound = 404,
  UserDuplicate = 409,

  NoFileProvided = 400,
  FileNotFound = 404,
  FileDuplicate = 409,

  SharedFileNotFound = 404,

  AccessLogNotFound = 404,

  FileVersionNotFound = 404,

  ScanResultNotFound = 404,
  ScanTimeout = 408,
  FileInfected = 422,

  DirectoryNotFound = 404,
  FileNotAccessible = 403,
  FileCreationFailed = 500,
  FileReadWriteError = 500,
  InsufficientStorage = 507,

  BadRequest = 400,
  InvalidToken = 401,
  Forbidden = 403,
  UnsupportedMediaType = 415,
  MissingJwtSecret = 500,
  InternalServerError = 500,
}
