using Microsoft.EntityFrameworkCore;
using PSK.Server.Data.Entities;
using PSK.Server.Services;
using PSK.Server.Specifications.JobSpecifications;
using PSK.Server.Specifications.LabelSpecifications;
using PSK.Server.Specifications.BoardColumnSpecifications;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public interface IJobService : IGenericService<Job, JobCreate, JobUpdate>
{
    Task UpdateLabels(Job entity, UpdateLabels labels);
    Task MoveJobToColumnAsync(Guid jobId, Guid columnId);
}

public class JobService : GenericService<Job, JobCreate, JobUpdate>, IJobService
{
    private readonly GenericRepository<Job> _jobRepository;
    private readonly GenericRepository<BoardColumn> _columnRepository;
    private readonly ILabelService _labelService;

    public JobService(
        GenericRepository<Job> repository,
        GenericRepository<BoardColumn> columnRepository,
        ILabelService labelService) : base(repository)
    {
        _jobRepository = repository;
        _columnRepository = columnRepository;
        _labelService = labelService;
    }

    public override async Task OnCreatingAsync(Job entity, JobCreate create)
    {
        entity.BoardId = create.BoardId;

        if (create.ColumnId.HasValue)
        {
            var column = await _columnRepository.GetByIdAsync(create.ColumnId.Value);
            if (column != null)
            {
                entity.ColumnId = column.Id;
                entity.Status = column.Name;
            }
            else
            {
                entity.Status = "To Do"; 
            }
        }
        else
        {
            var columns = await _columnRepository.ListAsync(new GetBoardColumnsByBoardIdSpec(create.BoardId));
            var todoColumn = columns.FirstOrDefault(c => c.Name.Equals("To Do", StringComparison.OrdinalIgnoreCase));

            if (todoColumn != null)
            {
                entity.ColumnId = todoColumn.Id;
                entity.Status = todoColumn.Name;
            }
            else
            {
                entity.Status = "To Do";
            }
        }
    }

    public override async Task OnUpdatingAsync(Job entity, JobUpdate update)
    {
        _repository.UpdateVersion(entity, update.Version);

        if (update.ColumnPosition.HasValue)
        {
            entity.ColumnPosition = update.ColumnPosition.Value;
        }

        if (update.ColumnId.HasValue && update.ColumnId != entity.ColumnId)
        {
            var column = await _columnRepository.GetByIdAsync(update.ColumnId.Value);
            if (column != null)
            {
                entity.ColumnId = column.Id;
                entity.Status = column.Name;
            }
        }
    }

    public async Task MoveJobToColumnAsync(Guid jobId, Guid columnId)
    {
        var job = await _jobRepository.GetByIdAsync(jobId);
        if (job == null)
        {
            throw new KeyNotFoundException("Job not found.");
        }

        var column = await _columnRepository.GetByIdAsync(columnId);
        if (column == null)
        {
            throw new KeyNotFoundException("Column not found.");
        }

        if (column.BoardId != job.BoardId)
        {
            throw new InvalidOperationException("Cannot move job to a column on a different board.");
        }

        job.ColumnId = columnId;
        job.Status = column.Name;

        await _jobRepository.UpdateAsync(job);
    }

    public async Task UpdateLabels(Job entity, UpdateLabels labels)
    {
        if (entity == null)
        {
            throw new KeyNotFoundException("Job not found.");
        }

        var newLabels = await _labelService.GetAllAsync(new GetLabelsByIdsSpec(labels.Labels));
        entity.Labels = newLabels;

        _repository.UpdateVersion(entity, labels.Version);
        await _jobRepository.UpdateAsync(entity);
    }

}