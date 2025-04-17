using Ardalis.Specification;
using PSK.Server.Data.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PSK.Server.Specifications.TeamSpecifications
{
    public class GetTeamByIdSpec : Specification<Team>, ISingleResultSpecification<Team>
    {
        public GetTeamByIdSpec(Guid teamId)
        {
            Query
                .Where(t => t.Id == teamId);
        }
    }
}
