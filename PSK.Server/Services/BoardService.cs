using PSK.Server.Data.Entities;

public interface IBoardService : IGenericService<Board, BoardCreate, BoardUpdate>
{
    // add new if needed
}

public class BoardService : GenericService<Board, BoardCreate, BoardUpdate>, IBoardService

{
    public BoardService(GenericRepository<Board> repository) : base(repository)
    {
    }

    public override async Task OnUpdatingAsync(Board entity, BoardUpdate update)
    {
        _repository.UpdateVersion(entity, update.Version);
    }
}
