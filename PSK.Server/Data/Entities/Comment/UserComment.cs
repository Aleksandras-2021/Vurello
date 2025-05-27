namespace PSK.Server.Data.Entities
{
    public class UserComment
    {
        public Guid Id { get; set; }
        public string Contents { get; set; }
        public Guid JobId { get; set; }
        public Job Job { get; set; }
        public Guid CreatorId { get; set; }
        public User Creator { get; set; }

        public uint Version { get; set; }
    }
}
