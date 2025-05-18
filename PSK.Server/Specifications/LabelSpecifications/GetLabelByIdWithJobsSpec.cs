using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.LabelSpecifications
{
    public class GetLabelByIdWithJobsSpec : Specification<Label>, ISingleResultSpecification<Label>
    {
        public GetLabelByIdWithJobsSpec(Guid labelId)
        {
            Query
                .Where(l => l.Id == labelId)
                .Include(l => l.Team)
                .Include(l => l.Jobs)
                    .ThenInclude(j => j.Board);
        }
    }
}