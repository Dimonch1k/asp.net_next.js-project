using backend_c_.DTO.Access;

namespace backend_c_.Service;

public interface IAccessLogService
{
  IEnumerable<AccessLogDto> GetAllAccessLogs( );
  IEnumerable<AccessLogDto> GetAccessLogsByFileId( int fileId );
  IEnumerable<AccessLogDto> GetAccessLogsByUserId( int userId );
  AccessLogDto GetAccessLogById( int id );
  AccessLogDto CreateAccessLog( CreateAccessLogDto createAccessLogDto );
  AccessLogDto UpdateAccessLog( int id, UpdateAccessLogDto updateAccessLogDto );
  AccessLogDto DeleteAccessLog( int id );
}
