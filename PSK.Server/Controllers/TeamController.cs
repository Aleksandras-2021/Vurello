using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;
using PSK.Server.Specifications.TeamSpecifications;
using PSK.Server.Authorization;

namespace PSK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/team")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IUserContext _userContext;
        public TeamController(ITeamService service, IUserContext userContext)
        {
            _teamService = service;
            _userContext = userContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserTeams()
        {
            var userId = _userContext.GetUserId(User);

            var teams = await _teamService.GetAllAsync(new GetUserTeamsSpec(userId));

            return Ok(teams);
        }


        [HttpGet("{teamId}")]
        [BelongsToTeam]  
        public async Task<IActionResult> GetTeam(Guid teamId)
        {
            var team = await _teamService.GetSingleAsync(new GetTeamByIdSpec(teamId));

            return Ok(team);
        }

        [HttpGet("{teamId}/members")]
        [BelongsToTeam]
        public async Task<IActionResult> GetTeamMembers(Guid teamId)
        {
            var teamMembers = await _teamService.GetAllAsync(new GetTeamMembersByIdSpec(teamId));

            return Ok(teamMembers);
        }

        [HttpGet("{teamId}/roles")]
        [BelongsToTeam]
        public async Task<IActionResult> GetTeamRoles(Guid teamId)
        {
            var teamRoles = await _teamService.GetAllAsync(new GetTeamRolesSpec(teamId));

            return Ok(teamRoles);
        }

        [HttpDelete("{teamId}/members/{userId}")]
        [HasPermission(PermissionName.TeamUsers)]
        public async Task<IActionResult> RemoveMember(Guid teamId, Guid userId)
        {
            var team = await _teamService.GetSingleAsync(new GetTeamByIdSpec(teamId));
            var currentUserId = _userContext.GetUserId(User);

            await _teamService.RemoveUserFromTeam(teamId, userId, currentUserId);
            return NoContent();

        }

        [HttpGet("{teamId}/contributions")]
        [BelongsToTeam]
        public async Task<IActionResult> GetContributions(Guid teamId)
        {
            var team = await _teamService.GetSingleAsync(new GetTeamByIdSpec(teamId));

            var contributionsDTO = _teamService.GetContributions(team);

            return Ok(contributionsDTO);
        }

        [HttpPut("{teamId}/assign")]
        [HasPermission(PermissionName.Roles)]
        public async Task<IActionResult> AssignRole(Guid teamId, [FromBody] AssignRole data)
        {
            await _teamService.AssignRole(teamId, data.RoleId, data.UserId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TeamCreate create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _teamService.CreateAsync(create);
            return Ok(entity);
        }

        [HttpPatch("{teamId}")]
        [HasPermission(PermissionName.Team)]
        public async Task<IActionResult> Update(Guid teamId, [FromBody] TeamUpdate update)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _teamService.UpdateAsync(teamId, update);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpDelete("{teamId}")]
        [HasPermission(PermissionName.Team)]
        public async Task<IActionResult> Delete(Guid teamId)
        {
            await _teamService.DeleteAsync(teamId);

            return NoContent();
        }
    }
}
