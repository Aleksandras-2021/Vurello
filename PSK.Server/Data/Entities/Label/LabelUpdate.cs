using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PSK.Server.Data.Entities
{
    public class LabelUpdate
    {
        [RequiredForSchema]
        [MinLength(3)]
        [MaxLength(20)]
        [Metadata("prompt", "Label text")]
        public string? Text { get; set; }

        [RequiredForSchema]
        [Metadata("prompt", "Text color")]
        [Metadata("colorPick", "true")]
        public string? TextColor { get; set; }

        [RequiredForSchema]
        [Metadata("prompt", "Background color")]
        [Metadata("colorPick", "true")]
        public string? BackgroundColor { get; set; }

        [Metadata("ignore", "true")]
        public uint Version { get; set; }
    }
}
