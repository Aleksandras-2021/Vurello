using PSK.Server.Misc;
using System.Text.Json.Serialization;

namespace PSK.Server.Data.Entities
{
    public class LabelUpdate
    {
        [Metadata("prompt", "Label text")]
        public string? Text { get; set; }

        [Metadata("prompt", "Text color")]
        [Metadata("colorPick", "true")]
        public string? TextColor { get; set; }

        [Metadata("prompt", "Background color")]
        [Metadata("colorPick", "true")]
        public string? BackgroundColor { get; set; }

        [Metadata("ignore", "true")]
        public Guid TeamId { get; set; }

    }
}
