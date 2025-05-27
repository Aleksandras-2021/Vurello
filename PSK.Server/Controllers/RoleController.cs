using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK.Controllers;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;
using PSK.Server.Services;
using PSK.Server.Specifications.RoleSpecifications;
using PSK.Server.Specifications.LabelSpecifications;
using PSK.Server.Authorization;

namespace PSK.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/role")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IUserContext _userContext;
        public RoleController(IRoleService service, IUserContext userContext)
        {
            _roleService = service;
            _userContext = userContext;
        }

        [HttpPut("{roleId}/permissions")]
        [HasPermission(PermissionName.Roles)]
        public async Task<IActionResult> UpdatePermissions(Guid roleId, [FromBody] UpdatePermissions permissions)
        {
            var role = await _roleService.GetSingleAsync(new GetRoleByIdSpec(roleId));
            await _roleService.UpdatePermissions(role, permissions);

            return Ok(role);
        }

        [HttpPost("{teamId}")]
        [HasPermission(PermissionName.Roles)]
        public async Task<IActionResult> Create([FromBody] RoleCreate create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _roleService.CreateAsync(create);
            return Ok(entity);
        }

        [HttpPatch("{roleId}")]
        [HasPermission(PermissionName.Roles)]
        public async Task<IActionResult> Update(Guid roleId, [FromBody] RoleUpdate update)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _roleService.UpdateAsync(roleId, update);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpDelete("{roleId}")]
        [HasPermission(PermissionName.Roles)]
        public async Task<IActionResult> Delete(Guid roleId)
        {
            await _roleService.DeleteAsync(roleId);

            return NoContent();
        }
    }
}
