using Ardalis.Specification;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PSK.Server.Data.Entities;
using PSK.Server.Specifications;
using PSK.Server.Specifications.LabelSpecifications;
using PSK.Server.Specifications.PermissionSpecifications;
using System.Data;

namespace PSK.Server.Services
{
    public interface IRoleService : IGenericService<Role, RoleCreate, RoleUpdate>
    {
        public Task UpdatePermissions(Role entity, UpdatePermissions permissions);
    }

    public class RoleService : GenericService<Role, RoleCreate, RoleUpdate>, IRoleService
    {
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IPermissionService _permissionService;
        private readonly UserManager<User> _userManager;


        public RoleService(IGenericRepository<Role> repository, IPermissionService permissionService, UserManager<User> userManager) : base(repository)
        {
            _roleRepository = repository;
            _permissionService = permissionService;
            _userManager = userManager;
        }
        public override async Task OnUpdatingAsync(Role entity, RoleUpdate update)
        {
            _repository.UpdateVersion(entity, update.Version);
        }

        public async Task UpdatePermissions(Role entity, UpdatePermissions permissions)
        {
            if (entity == null)
            {
                throw new KeyNotFoundException("Role not found.");
            }

            var newPermissions = await _permissionService.GetAllAsync(new GetPermissionsByIdsSpec(permissions.Permissions));

            entity.Permissions = newPermissions;

            _repository.UpdateVersion(entity, permissions.Version);

            await _roleRepository.UpdateAsync(entity);

        }

    }
}
