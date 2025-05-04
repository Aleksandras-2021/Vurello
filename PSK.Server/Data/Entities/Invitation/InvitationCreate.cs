using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class InvitationCreate
    {
        [Metadata("prompt", "Username to invite")]
        [MinLength(3)]
        [MaxLength(30)]
        public string RecipientUsername { get; set; } = string.Empty;

        [Metadata("ignore", "true")]
        public Guid TeamId { get; set; }
    }
}