namespace PSK.Server.Data.Entities
{
    public class Job
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public Guid BoardId { get; set; }
        public Board Board { get; set; }
        public String? AssignedMemberId { get; set; }
        public User? AssignedMember { get; set; }
    }
}