using Ardalis.Specification;
using PSK.Server.Data.Entities;

public class GetUserTeamsSpec : Specification<Team>
{
    public GetUserTeamsSpec(Guid userId)
    {
        Query.Where(t => t.Users.Any(u => u.Id == userId.ToString()));
    }
}
