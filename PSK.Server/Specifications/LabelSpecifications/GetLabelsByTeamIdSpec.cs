using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.LabelSpecifications
{
    public class GetLabelsByTeamIdSpec : Specification<Label>
    {
        public GetLabelsByTeamIdSpec(Guid teamId)
        {
            Query
                .Where(l => l.TeamId == teamId)
                .Include(l => l.Team)
                .Include(l => l.Jobs)
                    .ThenInclude(j => j.Board);
        }
    }
}