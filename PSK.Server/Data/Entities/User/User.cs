using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace PSK.Server.Data.Entities
{
    public class User : IdentityUser<Guid>
    {
        [JsonIgnore]
        public ICollection<Team> Teams { get; set; } = new List<Team>();

        [JsonIgnore]
        public ICollection<Team> CreatedTeams { get; set; } = new List<Team>();
        
        [JsonIgnore]
        public ICollection<Job> AssignedJobs { get; set; } = new List<Job>();




        //overrides

        [JsonIgnore]
        public override string? NormalizedUserName { get; set; }

        [JsonIgnore]
        public override string? Email { get; set; }

        [JsonIgnore]
        public override string? NormalizedEmail { get; set; }

        [JsonIgnore]
        public override bool EmailConfirmed { get; set; }

        [JsonIgnore]
        public override string? PasswordHash { get; set; }

        [JsonIgnore]
        public override string? SecurityStamp { get; set; }

        [JsonIgnore]
        public override string? ConcurrencyStamp { get; set; }

        [JsonIgnore]
        public override string? PhoneNumber { get; set; }

        [JsonIgnore]
        public override bool PhoneNumberConfirmed { get; set; }

        [JsonIgnore]
        public override bool TwoFactorEnabled { get; set; }

        [JsonIgnore]
        public override DateTimeOffset? LockoutEnd { get; set; }

        [JsonIgnore]
        public override bool LockoutEnabled { get; set; }

        [JsonIgnore]
        public override int AccessFailedCount { get; set; }
    }
}
