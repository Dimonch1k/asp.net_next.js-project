namespace backend_c_.Entity
{
    public class AccessLog
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public int UserId { get; set; }
        public string AccessType { get; set; }
        public DateTime AccessTime { get; set; } = DateTime.Now;

        public File File { get; set; }
        public User User { get; set; }
    }
}
