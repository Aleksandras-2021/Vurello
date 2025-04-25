using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.JobSpecifications
{
    public class GetJobsByBoardSpec : Specification<Job>
    {
        public GetJobsByBoardSpec(Guid boardId)
        {
            Query.Where(j => j.BoardId == boardId);
        }
    }
}