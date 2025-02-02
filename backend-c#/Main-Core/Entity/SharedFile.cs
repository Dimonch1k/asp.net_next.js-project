using backend_c_.Enums;

namespace backend_c_.Entity;

public class SharedFile
{
  public int Id { get; set; }
  public int FileId { get; set; }
  public int OwnerId { get; set; }
  public int SharedWithId { get; set; }
  public AccessType Permission { get; set; }
  public DateTime CreatedAt { get; set; }

  public MediaFile File { get; set; }
  public User Owner { get; set; }
  public User SharedWith { get; set; }
}
