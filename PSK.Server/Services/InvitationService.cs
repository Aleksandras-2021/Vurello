using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;

namespace PSK.Server.Services;
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
        if (recipient == null)
            throw new Exception("User not found");
            
        entity.RecipientUserId = recipient.Id;
        entity.SenderUserId = _userContext.GetUserId(_httpContextAccessor.HttpContext.User).ToString();
        entity.TeamId = create.TeamId;
        entity.CreatedAt = DateTime.UtcNow;
        entity.IsAccepted = false;
        entity.IsRejected = false;
    }
}