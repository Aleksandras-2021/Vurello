using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;
using PSK.Server.Specifications.TeamSpecifications;
using System.Security.Claims;


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
            var team = await _teamService.GetSingleAsync(new GetTeamByIdSpec(id));
            var members = team.Users.Select(u => new { u.Id, u.UserName }).ToList();

            return Ok(members);
        }
    }
}
