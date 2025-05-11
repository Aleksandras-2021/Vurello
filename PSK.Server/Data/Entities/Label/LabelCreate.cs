using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class LabelCreate
    {
        [Metadata("prompt", "Label text")]
        [Required]
        public string Text { get; set; }

        [Metadata("prompt", "Text color")]
        [Metadata("colorPick", "true")]
        [Required]
        public string TextColor { get; set; }

        [Metadata("prompt", "Background color")]
        [Metadata("colorPick", "true")]
        [Required]
        public string BackgroundColor { get; set; }

        [Metadata("ignore", "true")]
        public Guid TeamId { get; set; }
    }
}
