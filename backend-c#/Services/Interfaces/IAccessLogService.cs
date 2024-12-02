using backend_c_.DTO.Access;

namespace backend_c_.Services.Interfaces;

public interface IAccessLogService
{
  IEnumerable<AccessLogDto> FindAll( );
  AccessLogDto FindOne( int id );
  AccessLogDto Create( CreateAccessLogDto createAccessLogDto );
  AccessLogDto Update( int id, UpdateAccessLogDto updateAccessLogDto );
  bool Remove( int id );
  IEnumerable<AccessLogDto> FindAccessByFile( int fileId );
  IEnumerable<AccessLogDto> FindAccessByUser( int userId );
}
