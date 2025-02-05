using System.ComponentModel.DataAnnotations.Schema;
using backend_c_.Enums;

namespace backend_c_.Entity;

public class AccessLog
{
  public int Id { get; set; }
  public int FileId { get; set; }
  public int UserId { get; set; }
  public AccessType AccessType { get; set; }
  public DateTime AccessTime { get; set; }

  [ForeignKey( "FileId" )]
  public MediaFile File { get; set; }

  [ForeignKey( "UserId" )]
  public User User { get; set; }
}
