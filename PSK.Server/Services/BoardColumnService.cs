using PSK.Server.Data.Entities;
using PSK.Server.Specifications.BoardColumnSpecifications;
using PSK.Server.Specifications.JobSpecifications;

public interface IBoardColumnService : IGenericService<BoardColumn, BoardColumnCreate, BoardColumnUpdate>
{
    Task<List<BoardColumn>> GetBoardColumnsAsync(Guid boardId);
    Task ReorderColumnsAsync(Guid boardId, List<Guid> columnIds);
    Task DeleteColumnAsync(Guid columnId, Guid? targetColumnId);
    Task<BoardColumn> CreateDefaultColumnAsync(Guid boardId, string name, string color, int order, bool isDefault = false);
    Task EnsureDefaultColumnsExistAsync(Guid boardId);
}

public class BoardColumnService : GenericService<BoardColumn, BoardColumnCreate, BoardColumnUpdate>, IBoardColumnService
{
    private readonly GenericRepository<BoardColumn> _columnRepository;
    private readonly GenericRepository<Job> _jobRepository;
    private readonly GenericRepository<Board> _boardRepository;

    public BoardColumnService(
        GenericRepository<BoardColumn> repository,
        GenericRepository<Job> jobRepository,
        GenericRepository<Board> boardRepository) : base(repository)
    {
        _columnRepository = repository;
        _jobRepository = jobRepository;
        _boardRepository = boardRepository;
    }

    public override async Task OnCreatingAsync(BoardColumn entity, BoardColumnCreate create)
    {
        entity.BoardId = create.BoardId;

        if (create.Order <= 0)
        {
            var existingColumns = await _columnRepository.ListAsync(new GetBoardColumnsByBoardIdSpec(create.BoardId));

            int maxOrder = 0;
            foreach (var column in existingColumns)
            {
                if (column.Order > maxOrder)
                    maxOrder = column.Order;
            }

            if (existingColumns.Count > 0)
                entity.Order = maxOrder + 1;
            else
                entity.Order = 0;
        }
        else
        {
            entity.Order = create.Order;
        }

        await Task.CompletedTask;
    }

    public override async Task OnUpdatingAsync(BoardColumn entity, BoardColumnUpdate update)
    {
        _repository.UpdateVersion(entity, update.Version);

        if (update.Order.HasValue && update.Order.Value != entity.Order)
        {
            var columns = await _columnRepository.ListAsync(new GetBoardColumnsByBoardIdSpec(entity.BoardId));

            var filtered = new List<BoardColumn>();
            foreach (var column in columns)
            {
                if (column.Id != entity.Id)
                    filtered.Add(column);
            }

            var newOrder = Math.Min(update.Order.Value, filtered.Count);
            filtered.Insert(newOrder, entity);

            for (int i = 0; i < filtered.Count; i++)
            {
                filtered[i].Order = i;
                if (filtered[i].Id != entity.Id)
                {
                    await _columnRepository.UpdateAsync(filtered[i]);
                }
            }
        }

        await Task.CompletedTask;
    }

    public async Task<List<BoardColumn>> GetBoardColumnsAsync(Guid boardId)
    {
        var columns = await _columnRepository.ListAsync(new GetBoardColumnsByBoardIdSpec(boardId));
        return columns.OrderBy(c => c.Order).ToList();
    }

    public async Task ReorderColumnsAsync(Guid boardId, List<Guid> columnIds)
    {
        var columns = await _columnRepository.ListAsync(new GetBoardColumnsByBoardIdSpec(boardId));

        bool allIdsExist = columnIds.Count == columns.Count;
        if (allIdsExist)
        {
            foreach (var id in columnIds)
            {
                bool found = false;
                foreach (var column in columns)
                {
                    if (column.Id == id)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    allIdsExist = false;
                    break;
                }
            }
        }

        if (!allIdsExist)
        {
            throw new ArgumentException("Invalid column IDs provided for reordering");
        }

        for (int i = 0; i < columnIds.Count; i++)
        {
            BoardColumn column = null;
            foreach (var c in columns)
            {
                if (c.Id == columnIds[i])
                {
                    column = c;
                    break;
                }
            }

            if (column != null)
            {
                column.Order = i;
                await _columnRepository.UpdateAsync(column);
            }
        }
    }

    public async Task DeleteColumnAsync(Guid columnId, Guid? targetColumnId)
    {
        var column = await _columnRepository.GetByIdAsync(columnId);
        if (column == null)
        {
            throw new KeyNotFoundException("Column not found");
        }

        if (column.IsDefault)
        {
            throw new InvalidOperationException("Cannot delete default columns");
        }

        var jobs = await _jobRepository.ListAsync(new GetJobsByColumnIdSpec(columnId));

        if (targetColumnId.HasValue)
        {
            var targetColumn = await _columnRepository.GetByIdAsync(targetColumnId.Value);
            if (targetColumn == null)
            {
                throw new KeyNotFoundException("Target column not found");
            }

            foreach (var job in jobs)
            {
                job.ColumnId = targetColumnId.Value;
                job.Status = targetColumn.Name;
                await _jobRepository.UpdateAsync(job);
            }
        }
        else if (jobs.Any())
        {
            throw new InvalidOperationException("Cannot delete column with jobs without specifying a target column");
        }

        await _columnRepository.DeleteAsync(column);

        var remaining = await _columnRepository.ListAsync(new GetBoardColumnsByBoardIdSpec(column.BoardId));
        for (int i = 0; i < remaining.Count; i++)
        {
            if (remaining[i].Order != i)
            {
                remaining[i].Order = i;
                await _columnRepository.UpdateAsync(remaining[i]);
            }
        }
    }

    public async Task<BoardColumn> CreateDefaultColumnAsync(Guid boardId, string name, string color, int order, bool isDefault = false)
    {
        var column = new BoardColumn
        {
            BoardId = boardId,
            Name = name,
            Color = color,
            Order = order,
            IsDefault = isDefault
        };

        await _columnRepository.AddAsync(column);
        return column;
    }

    public async Task EnsureDefaultColumnsExistAsync(Guid boardId)
    {
        var columns = await _columnRepository.ListAsync(new GetBoardColumnsByBoardIdSpec(boardId));

        var toDoExists = columns.Any(c => c.Name == "To Do" && c.IsDefault);
        var inProgressExists = columns.Any(c => c.Name == "In Progress" && c.IsDefault);
        var doneExists = columns.Any(c => c.Name == "Done" && c.IsDefault);

        if (!toDoExists)
        {
            await CreateDefaultColumnAsync(boardId, "To Do", "#1890ff", 0, true);
        }

        if (!inProgressExists)
        {
            await CreateDefaultColumnAsync(boardId, "In Progress", "#faad14", 1, true);
        }

        if (!doneExists)
        {
            await CreateDefaultColumnAsync(boardId, "Done", "#52c41a", 2, true);
        }
    }
}