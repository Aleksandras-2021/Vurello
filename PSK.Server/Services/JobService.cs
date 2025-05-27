using Microsoft.EntityFrameworkCore;
using PSK.Server.Data.Entities;
using PSK.Server.Services;
using PSK.Server.Specifications.JobSpecifications;
using PSK.Server.Specifications.LabelSpecifications;
using PSK.Server.Specifications.BoardColumnSpecifications;
using PSK.Server.Misc;
using Microsoft.AspNetCore.Identity;

public interface IJobService : IGenericService<Job, JobCreate, JobUpdate>
{
    Task UpdateLabels(Job entity, UpdateLabels labels);
    Task MoveJobToColumnAsync(Guid jobId, Guid columnId);
    Task MoveJobToBoardAsync(Guid jobId, Guid targetBoardId);
}

public class JobService : GenericService<Job, JobCreate, JobUpdate>, IJobService
{
    private readonly IGenericRepository<Job> _jobRepository;
    private readonly ILabelService _labelService;
    private readonly IGenericRepository<BoardColumn> _columnRepository;
    private readonly IGenericRepository<Board> _boardRepository;
    private readonly IUserContext _userContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;

    public JobService(
        IGenericRepository<Job> repository,
        ILabelService labelService,
        IGenericRepository<BoardColumn> columnRepository,
        IGenericRepository<Board> boardRepository,
        IUserContext userContext,
        IHttpContextAccessor httpContextAccessor,
        UserManager<User> userManager) : base(repository)
    {
        _jobRepository = repository;
        _columnRepository = columnRepository;
        _labelService = labelService;
        _boardRepository = boardRepository;
        _userContext = userContext;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
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
        
        var userId = _userContext.GetUserId(_httpContextAccessor.HttpContext.User);
        var changeMessages = new List<string>();

        // Handle column position
        if (update.ColumnPosition.HasValue)
        {
            entity.ColumnPosition = update.ColumnPosition.Value;
        }

        // Handle column change
        if (update.ColumnId.HasValue && update.ColumnId != entity.ColumnId)
        {
            var column = await _columnRepository.GetByIdAsync(update.ColumnId.Value);
            if (column != null)
            {
                entity.ColumnId = column.Id;
                entity.Status = column.Name;
            }
        }

        // Track field changes - only for fields that are explicitly provided and different
        
        // Name - only track if provided and different
        if (update.Name != null && entity.Name != update.Name)
        {
            changeMessages.Add($"Name changed from '{entity.Name}' to '{update.Name}'");
        }

        // Description - only track if provided and different
        if (update.Description != null && entity.Description != update.Description)
        {
            changeMessages.Add($"Description was updated");
        }

        // Status - only track if provided and different
        if (update.Status != null && entity.Status != update.Status)
        {
            changeMessages.Add($"Status changed from '{entity.Status}' to '{update.Status}'");
        }

        // Deadline - only track if provided and different
        if (update.Deadline != null && entity.Deadline != update.Deadline)
        {
            var oldDeadline = entity.Deadline?.ToString("yyyy-MM-dd HH:mm") ?? "Not set";
            var newDeadline = update.Deadline?.ToString("yyyy-MM-dd HH:mm") ?? "Not set";
            changeMessages.Add($"Deadline changed from '{oldDeadline}' to '{newDeadline}'");
        }

        // Assignment - only track if provided and different
        if (update.AssignedMemberId != null)
        {
            var entityAssignment = entity.AssignedMemberId == Guid.Empty ? (Guid?)null : entity.AssignedMemberId;
            var updateAssignment = update.AssignedMemberId == Guid.Empty ? (Guid?)null : update.AssignedMemberId;
            
            if (entityAssignment != updateAssignment)
            {
                var oldAssignedName = "Unassigned";
                var newAssignedName = "Unassigned";

                if (entityAssignment.HasValue)
                {
                    var oldUser = await _userManager.FindByIdAsync(entityAssignment.Value.ToString());
                    oldAssignedName = oldUser?.UserName ?? "Unknown User";
                }

                if (updateAssignment.HasValue)
                {
                    var newUser = await _userManager.FindByIdAsync(updateAssignment.Value.ToString());
                    newAssignedName = newUser?.UserName ?? "Unknown User";
                }

                if (oldAssignedName != newAssignedName)
                {
                    changeMessages.Add($"Assigned member changed from '{oldAssignedName}' to '{newAssignedName}'");
                }
            }
        }

        // Add history entry if there are changes
        if (changeMessages.Any())
        {
            var jobHistory = new JobHistory
            {
                ChangeMessage = string.Join("; ", changeMessages),
                JobId = entity.Id,
                Timestamp = DateTime.UtcNow,
                UserId = userId
            };

            entity.JobHistories.Add(jobHistory);
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

        var userId = _userContext.GetUserId(_httpContextAccessor.HttpContext.User);
        var changeMessages = new List<string>();

        var currentLabelNames = entity.Labels?.Select(l => l.Text).OrderBy(x => x).ToList() ?? new List<string>();
        var newLabels = await _labelService.GetAllAsync(new GetLabelsByIdsSpec(labels.Labels));
        var newLabelNames = newLabels.Select(l => l.Text).OrderBy(x => x).ToList();

        if (!currentLabelNames.SequenceEqual(newLabelNames))
        {
            var oldLabelsText = currentLabelNames.Any() ? string.Join(", ", currentLabelNames) : "No labels";
            var newLabelsText = newLabelNames.Any() ? string.Join(", ", newLabelNames) : "No labels";
            
            changeMessages.Add($"Labels changed from '{oldLabelsText}' to '{newLabelsText}'");
        }

        entity.Labels = newLabels;
        _repository.UpdateVersion(entity, labels.Version);

        if (changeMessages.Any())
        {
            var jobHistory = new JobHistory
            {
                ChangeMessage = string.Join("; ", changeMessages),
                JobId = entity.Id,
                Timestamp = DateTime.UtcNow,
                UserId = userId
            };

            entity.JobHistories.Add(jobHistory);
        }

        await _jobRepository.UpdateAsync(entity);
    }

    public async Task MoveJobToBoardAsync(Guid jobId, Guid targetBoardId)
    {
        var job = await _jobRepository.GetByIdAsync(jobId);
        if (job == null)
        {
            throw new KeyNotFoundException("Job not found.");
        }

        var targetBoard = await _boardRepository.GetByIdAsync(targetBoardId);
        if (targetBoard == null)
        {
            throw new KeyNotFoundException("Target board not found.");
        }

        if (job.BoardId == targetBoardId)
        {
            throw new InvalidOperationException("Job is already on the target board.");
        }

        var targetColumns = await _columnRepository.ListAsync(new GetBoardColumnsByBoardIdSpec(targetBoardId));
        var defaultColumn = targetColumns.FirstOrDefault(c => c.Name.Equals("To Do", StringComparison.OrdinalIgnoreCase) && c.IsDefault)
                           ?? targetColumns.OrderBy(c => c.Order).FirstOrDefault();

        if (defaultColumn == null)
        {
            throw new InvalidOperationException("Target board has no available columns.");
        }

        job.BoardId = targetBoardId;
        job.ColumnId = defaultColumn.Id;
        job.Status = defaultColumn.Name;
        job.ColumnPosition = 0;

        await _jobRepository.UpdateAsync(job);
    }
}