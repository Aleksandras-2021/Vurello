using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.TeamSpecifications
{
    public class GetTeamMembersByIdSpec : Specification<Team, User>
    {
        public GetTeamMembersByIdSpec(Guid teamId)
        {
            Query.Where(t => t.Id == teamId)
                 .SelectMany(t => t.Users);
        }
    }
}
