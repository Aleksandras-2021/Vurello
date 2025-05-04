using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class Team
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? CreatorId { get; set; }
        public User? Creator { get; set; }
        public ICollection<Board> Boards { get; set; } = new List<Board>();

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
