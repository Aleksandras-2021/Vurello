using Microsoft.EntityFrameworkCore;
using PSK.Server.Data.Entities;
using PSK.Server.Services;
using PSK.Server.Specifications.JobSpecifications;
using PSK.Server.Specifications.LabelSpecifications;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
public interface IJobService : IGenericService<Job, JobCreate, JobUpdate>
{
    public Task UpdateLabels(Job entity, UpdateLabels labels);

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

    public override async Task OnUpdatingAsync(Job entity, JobUpdate update)
    {
        _repository.UpdateVersion(entity, update.Version);
    }

    public async Task UpdateLabels(Job entity, UpdateLabels labels)
    {
        if(entity == null)
        {
            throw new KeyNotFoundException("Job not found.");
        }

        var newLabels = await _labelService.GetAllAsync(new GetLabelsByIdsSpec(labels.Labels));

        entity.Labels = newLabels;

        _repository.UpdateVersion(entity, labels.Version);

        await _jobRepository.UpdateAsync(entity);        

    }
}