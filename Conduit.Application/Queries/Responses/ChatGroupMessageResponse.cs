using Conduit.Application.Helpers;

namespace Conduit.Application.Queries.Responses
{
    public class ChatGroupMessageResponse
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public ChatUserResponse Sender { get; set; }
        public DateTime SentAt { get; set; }
        public ChatGroupMessageResponse? SourceMessage { get; set; }
        public bool IsDeleted => Content.Equals(AppParameters.DeletedMessage);
    }
}
