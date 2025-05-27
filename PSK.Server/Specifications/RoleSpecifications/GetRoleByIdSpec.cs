using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.RoleSpecifications
{
    public class GetRoleByIdSpec : Specification<Role>, ISingleResultSpecification<Role>
    {
        public GetRoleByIdSpec(Guid roleId)
        {
            Query.Where(r => r.Id == roleId)
                .Include(r => r.Permissions)
                .Include(r => r.UserTeamRoles);

        }
    }
}
