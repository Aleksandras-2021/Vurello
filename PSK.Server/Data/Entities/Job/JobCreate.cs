using PSK.Server.Misc;
namespace PSK.Server.Data.Entities
{
    public class JobCreate
    {
        [Metadata("prompt", "Job name")]
        public string Name { get; set; }

        [Metadata("prompt", "Job description")]
        [Metadata("richText", "true")]
        public string Description { get; set; }

        [Metadata("prompt", "Job status")]
        [Metadata("enum", "To Do,In Progress,Done")]
        public string Status { get; set; } = "To Do";

        [Metadata("ignore", "true")]
        public Guid BoardId { get; set; }

        [Metadata("prompt", "Assigned Member")]
        [Metadata("dropdown", "Assigned Member")]
        public Guid? AssignedMemberId { get; set; }

    }
}