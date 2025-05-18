using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PSK.Server.Data.Entities
{
    public class JobUpdate
    {
        [RequiredForSchema]
        [MinLength(3)]
        [MaxLength(50)]
        [Metadata("prompt", "Job name")]
        public string? Name { get; set; }

        [Metadata("prompt", "Job description")]
        [Metadata("richText", "true")]
        public string? Description { get; set; }

        [Metadata("prompt", "Job status")]
        [Metadata("enum", "To Do,In Progress,Done")]
        public string? Status { get; set; }

        [Metadata("prompt", "Deadline")]
        [Metadata("format", "datetime-local")]
        public DateTime? Deadline { get; set; }

        [Metadata("prompt", "Assigned Member")]
        [Metadata("dropdown", "Assigned Member")]
        public Guid? AssignedMemberId { get; set; }

        [Metadata("ignore", "true")]
        public uint Version { get; set; }


    }
}