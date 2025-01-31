using backend_c_.DTO.Access;

namespace backend_c_.Service;

public interface IAccessLogService
{
  IEnumerable<AccessLogDto> FindAll( );
  AccessLogDto FindOne( int id );
  AccessLogDto Create( CreateAccessLogDto createAccessLogDto );
  AccessLogDto Update( int id, UpdateAccessLogDto updateAccessLogDto );
  AccessLogDto Remove( int id );
  IEnumerable<AccessLogDto> FindAccessByFile( int fileId );
  IEnumerable<AccessLogDto> FindAccessByUser( int userId );
}
