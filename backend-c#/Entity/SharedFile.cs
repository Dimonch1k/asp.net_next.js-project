using System.ComponentModel.DataAnnotations.Schema;
using backend_c_.Enums;

namespace backend_c_.Entity;

public class SharedFile
{
  public int Id { get; set; }
  public required int FileId { get; set; }
  public required int OwnerId { get; set; }
  public required int SharedWithId { get; set; }
  public AccessType Permission { get; set; }
  public DateTime CreatedAt { get; set; }

  [ForeignKey( "FileId" )]
  public MediaFile? File { get; set; }

  [ForeignKey( "OwnerId" )]
  public User? Owner { get; set; }

  [ForeignKey( "SharedWithId" )]
  public User? SharedWith { get; set; }
}
