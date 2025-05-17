namespace Conduit.Application.Queries.Responses
{
    public class ChatGroupParticipantResponse
    {
        public ChatUserResponse User { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
    }
}
