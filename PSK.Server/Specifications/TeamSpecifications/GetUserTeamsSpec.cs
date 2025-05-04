using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.TeamSpecifications
{
    public class GetUserTeamsSpec : Specification<Team>
    {
        public GetUserTeamsSpec(Guid userId)
        {
          Query
              .Where(t => t.Users.Any(u => u.Id == userId))
              .Include(t => t.Boards);
        }
    }
}
