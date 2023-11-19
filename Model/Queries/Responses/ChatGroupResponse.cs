namespace Model.Queries.Responses
{
    public class ChatGroupResponse
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public IEnumerable<ChatUserResponse> Participants { get; set; }
    }
}
