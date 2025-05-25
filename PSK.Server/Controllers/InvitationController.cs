using Microsoft.AspNetCore.Authorization;
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
    private readonly IUserContext _userContext;

    public InvitationController(IInvitationService invitationService, IUserContext userContext) : base(invitationService)
    {
        _invitationService = invitationService;
        _userContext = userContext;
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