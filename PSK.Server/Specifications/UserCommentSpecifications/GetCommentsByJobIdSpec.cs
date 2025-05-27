using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.CommentSpecifications
{
    public class GetCommentsByJobIdSpec : Specification<UserComment>
    {
        public GetCommentsByJobIdSpec(Guid jobId)
        {
            Query
                .Where(c => c.JobId == jobId)
                .Include(c => c.Creator);
        }
    }
}
