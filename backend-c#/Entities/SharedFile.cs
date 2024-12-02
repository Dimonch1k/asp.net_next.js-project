namespace backend_c_.Entity;

public class SharedFile
{
	public int Id { get; set; }
	public int FileId { get; set; }
	public int OwnerId { get; set; }
	public int SharedWithId { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.Now;

	public MediaFile File { get; set; }
	public User Owner { get; set; }
	public User SharedWith { get; set; }
}
