using PSK.Server.Misc;

namespace PSK.Server.Data.Entities
{
    public class Board
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid TeamId { get; set; }
        public Team Team { get; set; }
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
        public ICollection<BoardColumn> Columns { get; set; } = new List<BoardColumn>();

        public uint Version { get; set; }

    }
}
