using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class AssignRole
    {
   
        [Metadata("prompt", "Select role")]
        [Metadata("dropdown", "Role")]
        public Guid RoleId { get; set; }

        [Metadata("ignore", "true")]
        public Guid UserId { get; set; }


    }
}
