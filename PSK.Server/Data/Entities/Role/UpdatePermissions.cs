namespace PSK.Server.Data.Entities
{
    public class UpdatePermissions
    {
        public List<Guid> Permissions { get; set; }
        public uint Version { get; set; }
    }
}
