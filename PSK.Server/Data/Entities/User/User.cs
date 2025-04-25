using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace PSK.Server.Data.Entities
{
    public class User : IdentityUser
    {
        [JsonIgnore]
        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}
