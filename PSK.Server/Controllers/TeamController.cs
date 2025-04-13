using Microsoft.AspNetCore.Mvc;
using PSK.Server.Data.Entities;
using PSK.Server.Specifications.Generic;

namespace PSK.Controllers
{

    [ApiController]
    [Route("api/team")]
    public class TeamController : GenericController<Team, TeamCreate, TeamUpdate>
    {
        private readonly ITeamService _teamService;
        public TeamController(ITeamService service) : base(service)
        {
            _teamService = service;
        }

    }
}
