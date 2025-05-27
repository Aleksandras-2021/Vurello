using Ardalis.Specification;
using PSK.Server.Data.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PSK.Server.Specifications.TeamSpecifications
{
    public class GetTeamRolesSpec : Specification<Team>
    {
        public GetTeamRolesSpec(Guid teamId)
        {
            Query.Where(t => t.Id == teamId)
             .Include(t => t.Roles)
                 .ThenInclude(r => r.Permissions);
        }
    }
}
