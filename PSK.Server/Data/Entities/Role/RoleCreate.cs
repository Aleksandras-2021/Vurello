using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class RoleCreate
    {
        [Metadata("prompt", "Role name")]
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; }

        [Metadata("ignore", "true")]
        public Guid TeamId { get; set; }
    }
}
