namespace PSK.Server.Services;
using Microsoft.AspNetCore.Identity;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;
using PSK.Server.Specifications.InvitationSpecifications;

public interface IInvitationService : IGenericService<Invitation, InvitationCreate, InvitationUpdate>
{
}

public class InvitationService : GenericService<Invitation, InvitationCreate, InvitationUpdate>, IInvitationService
{
    private readonly UserManager<User> _userManager;
    private readonly IUserContext _userContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InvitationService(
        GenericRepository<Invitation> repository,
        UserManager<User> userManager,
        IUserContext userContext,
        IHttpContextAccessor httpContextAccessor) : base(repository)
    {
        _userManager = userManager;
        _userContext = userContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task OnCreatingAsync(Invitation entity, InvitationCreate create)
    {
        // Find recipient by username
        var recipient = await _userManager.FindByNameAsync(create.RecipientUsername);
        var senderUserId = _userContext.GetUserId(_httpContextAccessor.HttpContext.User).ToString();

        if (recipient == null)
        {
            throw new KeyNotFoundException("User not found");
        }
        if (recipient.Id == senderUserId)
        {
            throw new InvalidOperationException("Cannot invite yourself.");
        }

        var existingInvitations = await GetAllAsync(new GetUserInvitationsSpec(recipient.Id.ToString()));
        bool alreadyInvited = existingInvitations.Any(i =>
            i.TeamId == create.TeamId &&
            i.RecipientUserId == recipient.Id);

        if (alreadyInvited)
        {
            throw new InvalidOperationException("User already has an invitation.");
        }

        entity.RecipientUserId = recipient.Id;
        entity.SenderUserId = senderUserId;
        entity.TeamId = create.TeamId;
        entity.CreatedAt = DateTime.UtcNow;
        entity.IsAccepted = false;
        entity.IsRejected = false;
    }
}