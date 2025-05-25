using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class BoardColumnCreate
    {
        [Metadata("prompt", "Column name")]
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Metadata("prompt", "Column color")]
        [Metadata("colorPick", "true")]
        public string Color { get; set; } = "#1890ff";

        [Metadata("ignore", "true")]
        public Guid BoardId { get; set; }

        [Metadata("ignore", "true")]
        public int Order { get; set; }
    }
}