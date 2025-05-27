using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.TeamSpecifications
{
    public class GetTeamMemberRoles : Specification<Team>, ISingleResultSpecification<Team>
    {
        public GetTeamMemberRoles(Guid teamId)
        {
            Query.Where(t => t.Id == teamId)
                 .Include(t => t.UserTeamRoles);     
        }
    }
}
