using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK.Controllers;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;
using PSK.Server.Services;
using PSK.Server.Specifications.PermissionSpecifications;

namespace PSK.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/permission")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        public PermissionController(IPermissionService service)
        {
            _permissionService = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPermissions()
        {

            var permission = await _permissionService.GetAllAsync(new GetAllPermissionsSpec());

            return Ok(permission);
        }
    }
}
