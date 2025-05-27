using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;
using PSK.Controllers;
using PSK.Server.Services;
using PSK.Server.Specifications.InvitationSpecifications;
using PSK.Server.Authorization;

namespace PSK.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/invitation")]
public class InvitationController : ControllerBase
{
    private readonly IInvitationService _invitationService;
    private readonly IUserContext _userContext;

    public InvitationController(IInvitationService invitationService, IUserContext userContext)
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

    [HttpPost("{teamId}")]
    [HasPermission(PermissionName.TeamUsers)]
    public async Task<IActionResult> Create(Guid teamId, [FromBody] InvitationCreate create)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = await _invitationService.CreateAsync(create);
        return Ok(entity);
    }

    [HttpPatch("{invitationId}")]
    public async Task<IActionResult> Update(Guid invitationId, [FromBody] InvitationUpdate update)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updated = await _invitationService.UpdateAsync(invitationId, update);
        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }


}