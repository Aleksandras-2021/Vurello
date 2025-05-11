using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.LabelSpecifications
{
    public class GetLabelsByIdsSpec : Specification<Label>
    {
        public GetLabelsByIdsSpec(List<Guid> labelIds)
        {
            Query.Where(l => labelIds.Contains(l.Id));
        }
    }
}
