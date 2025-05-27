using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.PermissionSpecifications
{
    public class GetPermissionsByIdsSpec : Specification<Permission>
    {
        public GetPermissionsByIdsSpec(List<Guid> permissionIds)
        {
            Query.Where(p => permissionIds.Contains(p.Id));
        }
    }
}
