using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class BoardUpdate
    {
        [Metadata("prompt", "Board name")]
        [RequiredForSchema]
        [MinLength(3)]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Metadata("ignore", "true")]
        public uint Version { get; set; }


    }
}
