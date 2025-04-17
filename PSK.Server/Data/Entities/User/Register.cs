using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class Register
    {
        [Metadata("prompt", "Username")]

        public string Username { get; set; }

        [Metadata("prompt", "Password")]
        [Metadata("hidden", "true")]
        public string Password { get; set; }

    }
}
