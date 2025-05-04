using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class Login
    {
        [Metadata("prompt", "Username")]
        [Required] 
        [MinLength(3)]

        public string Username { get; set; }

        [Metadata("prompt", "Password")]
        [Metadata("hidden", "true")]
        [Required]
        [MinLength(6)]

        public string Password { get; set; }


    }
}
