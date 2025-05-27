namespace PSK.Server.Data.Entities
{
    public class UserTeamRole
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid TeamId { get; set; }
        public Team Team { get; set; }

        public Guid RoleId { get; set; }
        public Role Role { get; set; }

    }
}
