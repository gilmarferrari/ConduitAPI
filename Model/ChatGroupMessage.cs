using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class ChatGroupMessage
    {
        public int ID { get; set; }
        [Required, MinLength(1), MaxLength(1000)]
        public string Content { get; set; }
        public int SenderID { get; set; }
        public int ChatGroupID { get; set; }
        [Required]
        public DateTime SentAt { get; set; }
        public int? SourceMessageID { get; set; }
        public virtual User Sender { get; set; }
        public virtual ChatGroup ChatGroup { get; set; }
        public virtual ChatGroupMessage SourceMessage { get; set; }
    }
}
