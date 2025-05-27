using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class RoleUpdate
    {
        [Metadata("prompt", "Role name")]
        [RequiredForSchema]
        [MinLength(3)]
        [MaxLength(30)]
        public string? Name { get; set; }

        [Metadata("ignore", "true")]
        public uint Version { get; set; }
    }
}
