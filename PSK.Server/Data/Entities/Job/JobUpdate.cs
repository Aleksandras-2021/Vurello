using PSK.Server.Misc;
namespace PSK.Server.Data.Entities
{
    public class JobUpdate
    {
        [Metadata("prompt", "Job name")]
        public string? Name { get; set; }

        [Metadata("prompt", "Job description")]
        public string? Description { get; set; }

        [Metadata("prompt", "Job status")]
        [Metadata("enum", "To Do,In Progress,Done")]
        public string? Status { get; set; }

        [Metadata("prompt", "Assigned Member")]
        [Metadata("dropdown", "Assigned Member")]
        public Guid? AssignedMemberId { get; set; }
    }
}