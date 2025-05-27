namespace PSK.Server.Data.Entities
{
    public class Job
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public Guid? ColumnId { get; set; }
        public BoardColumn? Column { get; set; }
        public DateTime? Deadline { get; set; }
        public Guid BoardId { get; set; }
        public Board Board { get; set; }
        public Guid? AssignedMemberId { get; set; }
        public User? AssignedMember { get; set; }

        public ICollection<Label> Labels { get; set; } = new List<Label>();
        public ICollection<JobHistory> JobHistories { get; set; } = new List<JobHistory>();

        public uint Version { get; set; }
        public int ColumnPosition { get; set; } = 0;
    }
}