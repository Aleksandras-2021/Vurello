using PSK.Server.Misc;

namespace PSK.Server.Data.Entities
{
    public class Board : IHasId
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid TeamId { get; set; }
        public Team Team { get; set; }
    }
}
