namespace PSK.Server.Data.Entities
{
    public class JobHistory
    {
        public Guid Id { get; set; }
        public string ChangeMessage { get; set; }
        public Guid JobId { get; set; }
        public Job Job { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}