using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.TeamSpecifications
{
    public class GetTeamByIdWithUsersSpec : Specification<Team>, ISingleResultSpecification<Team>
    {
        public GetTeamByIdWithUsersSpec(Guid teamId)
        {
            Query
                .Where(t => t.Id == teamId)
                .Include(t => t.Users);
        }
    }
}