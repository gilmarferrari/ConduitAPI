using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Message
    {
        public int ID { get;set; }
        [Required, MinLength(1), MaxLength(1000)]
        public string Content { get; set; }
        public int SenderID { get;set; }
        public int RecipientID { get;set; }
        [Required]
        public DateTime SentAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public int? SourceMessageID { get; set; }
        public virtual User Sender { get; set; }
        public virtual User Recipient { get; set; }
        public virtual Message SourceMessage { get; set; }
    }
}
