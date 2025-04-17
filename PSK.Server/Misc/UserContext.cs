using System.Security.Claims;

namespace PSK.Server.Misc
{
    public interface IUserContext
    {
        Guid GetUserId(ClaimsPrincipal user);
    }

    public class UserContext : IUserContext
    {
        public Guid GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException();
            }

            return Guid.Parse(userIdClaim);
        }
    }

}
