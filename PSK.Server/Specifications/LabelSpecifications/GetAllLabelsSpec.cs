using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.LabelSpecifications
{
    public class GetAllLabelsSpec : Specification<Label>
    {
        public GetAllLabelsSpec()
        {
            Query
                .Include(l => l.Team)
                .Include(l => l.Jobs)
                    .ThenInclude(j => j.Board);
        }
    }
}