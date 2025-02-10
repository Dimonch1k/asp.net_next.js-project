using System.ComponentModel.DataAnnotations.Schema;
using backend_c_.Entity;

namespace backend_c_.Entity;

public class Notification
{
  public int Id { get; set; }
  public int? UserId { get; set; }
  public string? Message { get; set; }
  public DateTime CreatedAt { get; set; }

  [ForeignKey( "UserId" )]
  public User? User { get; set; }
}
