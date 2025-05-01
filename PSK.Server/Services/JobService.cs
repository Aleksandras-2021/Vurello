using PSK.Server.Data.Entities;
public interface IJobService : IGenericService<Job, JobCreate, JobUpdate>
{
}

public class JobService : GenericService<Job, JobCreate, JobUpdate>, IJobService
{
    private readonly GenericRepository<Job> _jobRepository;

    public JobService(GenericRepository<Job> repository) : base(repository)
    {
        _jobRepository = repository;
    }


    public override async Task OnCreatingAsync(Job entity, JobCreate create)
    {
        entity.Status = create.Status ?? "To Do";
        await Task.CompletedTask;
    }
}