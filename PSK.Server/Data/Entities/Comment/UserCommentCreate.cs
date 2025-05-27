using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities.Comment
{
    public class UserCommentCreate
    {
        [Metadata("prompt", "Contents")]
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Contents { get; set; }

        [Metadata("ignore", "true")]
        public Guid JobId { get; set; }

        [Metadata("ignore", "true")]
        public Guid CreatorId { get; set; }
    }
}
