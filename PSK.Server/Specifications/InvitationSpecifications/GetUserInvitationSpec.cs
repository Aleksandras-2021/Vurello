using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Specifications.InvitationSpecifications
{
    public class GetUserInvitationsSpec : Specification<Invitation>
    {
        public GetUserInvitationsSpec(string userId)
        {
            Query
                .Where(i => i.RecipientUserId == userId && !i.IsAccepted && !i.IsRejected)
                .Include(i => i.Team)
                .Include(i => i.Sender);
        }
    }
}