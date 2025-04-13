using PSK.Server.Misc;

namespace PSK.Server.Data.Entities
{
    public class BoardCreate
    {
        [Metadata("prompt", "Board name")]
        public string Name { get; set; }

        [Metadata("ignore", "true")]
        public Guid TeamId { get; set; }
    }
}
