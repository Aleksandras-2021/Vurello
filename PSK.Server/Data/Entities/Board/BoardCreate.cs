using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class BoardCreate
    {
        [Metadata("prompt", "Board name")]
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Metadata("ignore", "true")]
        public Guid TeamId { get; set; }
    }
}
