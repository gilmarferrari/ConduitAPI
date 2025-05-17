using System.ComponentModel.DataAnnotations;

namespace Conduit.Domain.Entities
{
    public class ChatGroupMessage
    {
        public int ID { get; set; }
        [Required, MinLength(1), MaxLength(1000)]
        public string Content { get; set; }
        public Guid SenderID { get; set; }
        public Guid ChatGroupID { get; set; }
        [Required]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public int? SourceMessageID { get; set; }
        public virtual User Sender { get; set; }
        public virtual ChatGroup ChatGroup { get; set; }
        public virtual ChatGroupMessage SourceMessage { get; set; }
    }
}
