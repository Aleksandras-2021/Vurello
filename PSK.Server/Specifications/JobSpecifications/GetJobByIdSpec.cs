using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.JobSpecifications
{
    public class GetJobByIdSpec : Specification<Job>, ISingleResultSpecification<Job>
    {
        public GetJobByIdSpec(Guid jobId) 
        {
            Query
                .Where(j => j.Id == jobId)
                .Include(j => j.Labels);
        }
    }
}
