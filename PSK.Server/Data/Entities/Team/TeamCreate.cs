using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class TeamCreate
    {
        [Metadata("prompt", "Team name")]
        public string Name { get; set; }

        [Metadata("ignore", "true")]
        public string UserId { get; set; }

    }
}
