using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;
namespace PSK.Server.Data.Entities
{
    public class JobCreate
    {
        [Metadata("prompt", "Job name")]
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Metadata("prompt", "Job description")]
        [Metadata("richText", "true")]
        public string? Description { get; set; }

        [Metadata("prompt", "Job status")]
        [Metadata("enum", "To Do,In Progress,Done")]
        public string Status { get; set; } = "To Do";

        [Metadata("prompt", "Deadline")]
        [Metadata("format", "datetime-local")]
        public DateTime? Deadline { get; set; }

        [Metadata("ignore", "true")]
        public Guid BoardId { get; set; }

        [Metadata("prompt", "Assigned Member")]
        [Metadata("dropdown", "Assigned Member")]
        public Guid? AssignedMemberId { get; set; }

    }
}