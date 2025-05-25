using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class BoardColumnUpdate
    {
        [Metadata("prompt", "Column name")]
        [RequiredForSchema]
        [MinLength(2)]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Metadata("prompt", "Column color")]
        [Metadata("colorPick", "true")]
        public string? Color { get; set; }

        [Metadata("ignore", "true")]
        public int? Order { get; set; }

        [Metadata("ignore", "true")]
        public uint Version { get; set; }
    }
}