using PSK.Server.Data.Entities;
using PSK.Server.Data.Entities.Comment;
using PSK.Server.Specifications.CommentSpecifications;

public interface IUserCommentService
{
    Task<IEnumerable<UserComment>> GetCommentsByJobIdAsync(Guid jobId);
    Task<UserComment> CreateCommentAsync(UserCommentCreate commentDto);
}

public class UserCommentService : IUserCommentService
{
    private readonly IGenericRepository<UserComment> _commentRepository;
    private readonly IGenericRepository<Job> _jobRepository;

    public UserCommentService(IGenericRepository<UserComment> commentRepository, IGenericRepository<Job> jobRepository)
    {
        _commentRepository = commentRepository;
        _jobRepository = jobRepository;
    }

    public async Task<IEnumerable<UserComment>> GetCommentsByJobIdAsync(Guid jobId)
    {
        var spec = new GetCommentsByJobIdSpec(jobId);
        return await _commentRepository.ListAsync(spec);
    }

    public async Task<UserComment> CreateCommentAsync(UserCommentCreate commentDto)
    {
        var comment = new UserComment
        {
            Id = Guid.NewGuid(),
            Contents = commentDto.Contents,
            JobId = commentDto.JobId,
            CreatorId = commentDto.CreatorId,
            Version = 1
        };

        await _commentRepository.AddAsync(comment);
        return comment;
    }
}
