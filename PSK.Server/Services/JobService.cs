using Microsoft.EntityFrameworkCore;
using PSK.Server.Data.Entities;
using PSK.Server.Services;
using PSK.Server.Specifications.JobSpecifications;
using PSK.Server.Specifications.LabelSpecifications;
public interface IJobService : IGenericService<Job, JobCreate, JobUpdate>
{
    public Task UpdateLabels(Job entity, List<Guid> labelsIds);

}

public class JobService : GenericService<Job, JobCreate, JobUpdate>, IJobService
{
    private readonly GenericRepository<Job> _jobRepository;
    private readonly ILabelService _labelService;

    public JobService(GenericRepository<Job> repository, ILabelService labelService) : base(repository)
    {
        _jobRepository = repository;
        _labelService = labelService;
    }


    public override async Task OnCreatingAsync(Job entity, JobCreate create)
    {
        entity.Status = create.Status ?? "To Do";
        await Task.CompletedTask;
    }

    public async Task UpdateLabels(Job entity, List<Guid> labelsIds)
    {
        if(entity == null)
        {
            throw new KeyNotFoundException("Job not found.");
        }

        var newLabels = await _labelService.GetAllAsync(new GetLabelsByIdsSpec(labelsIds));

        entity.Labels = newLabels;

        await _jobRepository.UpdateAsync(entity);

    }
}