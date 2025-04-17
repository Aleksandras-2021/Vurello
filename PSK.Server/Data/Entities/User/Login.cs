using PSK.Server.Misc;
using System.ComponentModel.DataAnnotations;

namespace PSK.Server.Data.Entities
{
    public class Login
    {
        [Metadata("prompt", "Username")]

        public string Username { get; set; }

        [Metadata("prompt", "Password")]
        [Metadata("hidden", "true")]

        public string Password { get; set; }


    }
}
