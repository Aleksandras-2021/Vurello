using Ardalis.Specification;
using Mapster;
using PSK.Server.Data.Entities;

public interface ITeamService : IGenericService<Team, TeamCreate, TeamUpdate>
{
    // add new if needed
}

public class TeamService : GenericService<Team, TeamCreate, TeamUpdate>, ITeamService
{
    public TeamService(GenericRepository<Team> repository) : base(repository)
    {
    }


}
