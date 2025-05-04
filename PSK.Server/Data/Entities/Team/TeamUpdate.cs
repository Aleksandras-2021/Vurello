using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class TeamUpdate
    {
        [MinLength(3)]
        [MaxLength(30)]
        public string? Name { get; set; }

    }
}
