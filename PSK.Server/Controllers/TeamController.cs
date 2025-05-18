using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;
using PSK.Server.Specifications.TeamSpecifications;

namespace PSK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/team")]
    public class TeamController : GenericController<Team, TeamCreate, TeamUpdate>
    {
        private readonly ITeamService _teamService;
        private readonly IUserContext _userContext;
        public TeamController(ITeamService service, IUserContext userContext) : base(service)
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeam(Guid id)
        {
            var team = await _teamService.GetSingleAsync(new GetTeamByIdSpec(id));

            return Ok(team);
        }

        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetTeamMembers(Guid id)
        {
            var teamMembers = await _teamService.GetAllAsync(new GetTeamMembersByIdSpec(id));

            return Ok(teamMembers);
        }

        [HttpDelete("{teamId}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(Guid teamId, Guid userId)
        {
            var team = await _teamService.GetSingleAsync(new GetTeamByIdSpec(teamId));
            var currentUserId = _userContext.GetUserId(User);
            if (team.CreatorId != currentUserId)
            {
                return Forbid("Only the team creator can remove members.");
            }

            if (userId == team.CreatorId)
            {
                return BadRequest("Cannot remove creator from team.");
            }

            try
            {
                await _teamService.RemoveUserFromTeam(teamId, userId);
                return NoContent();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("{teamId}/contributions")]
        public async Task<IActionResult> GetContributions(Guid teamId)
        {
            var team = await _teamService.GetSingleAsync(new GetTeamByIdSpec(teamId));

            var contributionsDTO = _teamService.GetContributions(team);

            return Ok(contributionsDTO);
        }
    }
}
