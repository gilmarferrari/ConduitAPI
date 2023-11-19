namespace Model.Queries.Responses
{
    public class ChatGroupMessageResponse
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public ChatUserResponse Sender { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsDeleted { get; set; }
        public ChatGroupMessageResponse? SourceMessage { get; set; }
    }
}
