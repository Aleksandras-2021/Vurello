using System;

namespace PSK.Server.Data.Entities
{
    public class Invitation
    {
         public Guid Id { get; set; }
         public Guid? RecipientUserId { get; set; }
         public Guid? SenderUserId { get; set; }
         
         public Guid TeamId { get; set; }
         public DateTime CreatedAt { get; set; }
         public bool IsAccepted { get; set; }
         public bool IsRejected { get; set; }
         
         public User? Recipient { get; set; }
         public User? Sender { get; set; }
         public Team? Team { get; set; }
    }
}
