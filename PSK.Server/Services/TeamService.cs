using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;
using PSK.Server.Specifications.TeamSpecifications;

public interface ITeamService : IGenericService<Team, TeamCreate, TeamUpdate>
{
    void AddUserToTeam(Team team, User user);
    Task RemoveUserFromTeam(Guid teamId, Guid userId);
}

public class TeamService : GenericService<Team, TeamCreate, TeamUpdate>, ITeamService
{
    private readonly IUserContext _userContext;
    private readonly GenericRepository<Team> _teamRepository;
    private readonly UserManager<User> _userManager;

    public TeamService(GenericRepository<Team> repository, IUserContext userContext, UserManager<User> userManager) : base(repository)
    {
        _teamRepository = repository;
        _userContext = userContext;
        _userManager = userManager;
    }
    public override async Task OnCreatingAsync(Team entity, TeamCreate create)
    {
        var user = await _userManager.FindByIdAsync(create.UserId);
        entity.CreatorId = Guid.Parse(create.UserId);
        AddUserToTeam(entity, user);
    }

    public override async Task<bool> AuthorizeAsync(Guid id, ClaimsPrincipal user)
    {
        var userId = _userContext.GetUserId(user).ToString();
        var team = await _repository.SingleOrDefaultAsync(
            new GetTeamForAuthorizationSpec(id));
        if (team == null)
        {
            throw new KeyNotFoundException("Team wiht ID {id} not found.");
        }
        return team.CreatorId.ToString() == userId;
    }
    
    public void AddUserToTeam(Team team, User user)
    {
        if (team == null)
        {
            throw new ArgumentNullException(nameof(team), "Team must not be null.");
        }

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User must not be null.");
        }

        if (team.Users.Any(u => u.Id == user.Id))
        {
            throw new InvalidOperationException($"User with ID {user.Id} is already a member of the team.");
        }

        team.Users.Add(user);
    }

    public async Task RemoveUserFromTeam(Guid teamId, Guid userId)
    {
        var team = await _repository.SingleOrDefaultAsync(new GetTeamByIdSpec(teamId));
        if (team == null)
        {
            throw new KeyNotFoundException($"Team with ID {teamId} not found.");
        }

        if (team.CreatorId == userId.ToString())
        {
            throw new InvalidOperationException("Cannot remove creator from team.");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }
        
        var userInTeam = team.Users.FirstOrDefault(u => u.Id == userId.ToString());
        if (userInTeam == null)
        {
            throw new InvalidOperationException($"User with ID {userId} is not a member of the team.");
        }
        
        team.Users.Remove(userInTeam);
        await _teamRepository.UpdateAsync(team);
    }
}
