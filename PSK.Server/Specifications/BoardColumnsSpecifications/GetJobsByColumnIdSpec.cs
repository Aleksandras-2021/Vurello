using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.JobSpecifications
{
    public class GetJobsByColumnIdSpec : Specification<Job>
    {
        public GetJobsByColumnIdSpec(Guid columnId)
        {
            Query.Where(job => job.ColumnId == columnId);
        }
    }
}