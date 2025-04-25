using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.JobSpecifications
{
    public class GetAllJobsSpec : Specification<Job>
    {
        public GetAllJobsSpec()
        {
            Query
                .Include(j => j.Board);
        }
    }
}