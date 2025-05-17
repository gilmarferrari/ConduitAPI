using Conduit.Application.Queries.Responses;

namespace Conduit.Application.Queries.Responses
{
    public class MessageResponse
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public ChatUserResponse Sender { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsDeleted { get; set; }
        public MessageResponse? SourceMessage { get; set; }
    }
}
