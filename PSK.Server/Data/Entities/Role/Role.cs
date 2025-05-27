namespace PSK.Server.Data.Entities
{
    public class Role
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid TeamId { get; set; }
        public Team Team { get; set; }

        public ICollection<UserTeamRole> UserTeamRoles { get; set; } = new List<UserTeamRole>();


        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        public uint Version { get; set; }


    }
}
