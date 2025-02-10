using backend_c_.DTO.Access;

namespace backend_c_.Service;

public interface IAccessLogService
{
  IEnumerable<AccessLogDto> GetAllAccessLogs( );
  Task<AccessLogDto> GetAccessLogById( int id );
  Task<IEnumerable<AccessLogDto>> GetAccessLogsBySharedFileId( int fileId );
  Task<IEnumerable<AccessLogDto>> GetAccessLogsByUserId( int userId );
  Task<AccessLogDto> CreateAccessLog( CreateAccessLogDto createAccessLogDto );
  Task<AccessLogDto> UpdateAccessLog( int id, UpdateAccessLogDto updateAccessLogDto );
  Task<AccessLogDto> DeleteAccessLog( int id );
}
