using PSK.Server.Misc;

namespace PSK.Server.Data.Entities
{
    public class InvitationCreate
    {
        [Metadata("prompt", "Username to invite")]
        public string RecipientUsername { get; set; } = string.Empty;
        
        [Metadata("ignore", "true")]
        public Guid TeamId { get; set; }
    }
}