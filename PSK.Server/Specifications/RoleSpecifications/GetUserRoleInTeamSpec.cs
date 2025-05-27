using Ardalis.Specification;
using PSK.Server.Data.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PSK.Server.Specifications
{
    public class GetUserTeamRoleInRoleSpec : Specification<Role, UserTeamRole>, ISingleResultSpecification<Role, UserTeamRole>
    {
        public GetUserTeamRoleInRoleSpec(Guid teamId, Guid userId)
        {
            Query
                .Where(r => r.TeamId == teamId && r.UserTeamRoles.Any(utr => utr.UserId == userId))
                .Select(r => r.UserTeamRoles.First(utr => utr.UserId == userId));
        }
    }

}
