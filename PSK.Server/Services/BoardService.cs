using PSK.Server.Data.Entities;

public interface IBoardService : IGenericService<Board, BoardCreate, BoardUpdate>
{
    Task<Board> CreateBoardWithDefaultColumnsAsync(BoardCreate create);
}

public class BoardService : GenericService<Board, BoardCreate, BoardUpdate>, IBoardService
{
    private readonly IBoardColumnService _columnService;

    public BoardService(IGenericRepository<Board> repository, IBoardColumnService columnService) : base(repository)
    {
        _columnService = columnService;
    }

    public override async Task OnCreatingAsync(Board entity, BoardCreate create)
    {
        entity.TeamId = create.TeamId;
        await base.OnCreatingAsync(entity, create);
    }

    public async Task<Board> CreateBoardWithDefaultColumnsAsync(BoardCreate create)
    {
        var board = await base.CreateAsync(create);

        
        await _columnService.EnsureDefaultColumnsExistAsync(board.Id);

        return board;
    }
    public override async Task OnUpdatingAsync(Board entity, BoardUpdate update)
    {
        _repository.UpdateVersion(entity, update.Version);
    }
}