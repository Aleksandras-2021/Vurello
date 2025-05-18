using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class TeamUpdate
    {
        [RequiredForSchema]
        [Metadata("prompt", "Team name")]
        [MinLength(3)]
        [MaxLength(30)]
        public string? Name { get; set; }

        [Metadata("ignore", "true")]
        public uint Version { get; set; }

    }
}
