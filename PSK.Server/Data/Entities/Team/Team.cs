using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class Team : IHasId
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<Board> Boards { get; set; }
    }
}
