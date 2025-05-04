using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;
using PSK.Controllers;
using PSK.Server.Services;
using PSK.Server.Specifications.InvitationSpecifications;

namespace PSK.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/invitation")]
public class InvitationController : GenericController<Invitation, InvitationCreate, InvitationUpdate>
{
    private readonly IInvitationService _invitationService;
    private readonly ITeamService _teamService;
    private readonly IUserContext _userContext;
    private readonly UserManager<User> _userManager;
    private readonly GenericRepository<Team> _teamRepository;

    public InvitationController(
        IInvitationService invitationService,
        ITeamService teamService,
        IUserContext userContext,
        UserManager<User> userManager,
        GenericRepository<Team> teamRepository)
        : base(invitationService)
    {
        _invitationService = invitationService;
        _teamService = teamService;
        _userContext = userContext;
        _userManager = userManager;
        _teamRepository = teamRepository;
    }

    [HttpGet("inbox")]
    public async Task<IActionResult> GetInvitations()
    {
        var userId = _userContext.GetUserId(User);
        var spec = new GetUserInvitationsSpec(userId);
        var invitations = await _invitationService.GetAllAsync(spec);
        return Ok(invitations);
    }

}