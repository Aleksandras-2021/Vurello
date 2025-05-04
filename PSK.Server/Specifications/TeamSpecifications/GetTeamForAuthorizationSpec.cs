using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.TeamSpecifications
{
    public class GetTeamForAuthorizationSpec : Specification<Team>, ISingleResultSpecification<Team>
    {
        public GetTeamForAuthorizationSpec(Guid teamId)
        {
            Query.Where(t => t.Id == teamId);
        }
    }
}