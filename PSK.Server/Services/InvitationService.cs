namespace PSK.Server.Services;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;
using PSK.Server.Specifications.InvitationSpecifications;
using PSK.Server.Specifications.TeamSpecifications;

public interface IInvitationService : IGenericService<Invitation, InvitationCreate, InvitationUpdate>
{
}

public class InvitationService : GenericService<Invitation, InvitationCreate, InvitationUpdate>, IInvitationService
{
    private readonly UserManager<User> _userManager;
    private readonly IUserContext _userContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITeamService _teamService;

    public InvitationService(
        GenericRepository<Invitation> repository,
        UserManager<User> userManager,
        IUserContext userContext,
        IHttpContextAccessor httpContextAccessor,
        ITeamService teamService) : base(repository)
    {
        _userManager = userManager;
        _userContext = userContext;
        _httpContextAccessor = httpContextAccessor;
        _teamService = teamService;
    }

    public override async Task OnCreatingAsync(Invitation entity, InvitationCreate create)
    {
        var recipient = await _userManager.FindByNameAsync(create.RecipientUsername);
        var senderUserId = _userContext.GetUserId(_httpContextAccessor.HttpContext.User);

        if (recipient == null)
        {
            throw new KeyNotFoundException("User not found");
        }
        if (recipient.Id == senderUserId)
        {
            throw new InvalidOperationException("Cannot invite yourself.");
        }

        var existingInvitations = await GetAllAsync(new GetUserInvitationsSpec(recipient.Id));
        bool alreadyInvited = existingInvitations.Any(i =>
            i.TeamId == create.TeamId &&
            i.RecipientUserId == recipient.Id);

        if (alreadyInvited)
        {
            throw new InvalidOperationException("User already has an invitation.");
        }


        var team = await _teamService.GetSingleAsync(new GetTeamByIdSpec(entity.TeamId));
        if (team == null)
        {
            throw new KeyNotFoundException("Team not found");
        }
        if (team.Users.Any(u => u.Id == recipient.Id))
        {
            throw new InvalidOperationException("User already in the team.");
        }

        entity.RecipientUserId = recipient.Id;
        entity.SenderUserId = senderUserId;
        entity.TeamId = create.TeamId;
        entity.CreatedAt = DateTime.UtcNow;
        entity.IsAccepted = false;
        entity.IsRejected = false;
    }
    public override async Task OnUpdatingAsync(Invitation entity, InvitationUpdate update)
    {
        if(update.IsAccepted != null && update.IsAccepted == true)
        {
            var userId = _userContext.GetUserId(_httpContextAccessor.HttpContext.User);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var team = await _teamService.GetSingleAsync(new GetTeamByIdSpec(entity.TeamId));
            if (team == null)
            {
                throw new KeyNotFoundException("Team not found");
            }

            _teamService.AddUserToTeam(team, user);
        }

    }

}