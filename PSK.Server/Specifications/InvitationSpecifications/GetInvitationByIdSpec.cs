using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.InvitationSpecifications
{
    public class GetInvitationByIdSpec : Specification<Invitation>, ISingleResultSpecification<Invitation>
    {
        public GetInvitationByIdSpec(Guid invitationId)
        {
            Query
                .Where(i => i.Id == invitationId)
                .Include(i => i.Team)
                .Include(i => i.Sender)
                .Include(i => i.Recipient);
        }
    }
}