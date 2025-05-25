using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class BoardColumn
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; } = "#1890ff";

        public int Order { get; set; }

        public Guid BoardId { get; set; }

        public Board Board { get; set; }

        public ICollection<Job> Jobs { get; set; } = new List<Job>();

        public uint Version { get; set; }

        public bool IsDefault { get; set; } = false;
    }
}