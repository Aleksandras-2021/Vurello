using Microsoft.AspNetCore.Identity;

namespace PSK.Server.Data.Entities
{
    public class User : IdentityUser
    {
        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}
