using Conduit.Domain.Entities;

namespace Conduit.Application.Queries.Responses
{
    public class ChatGroupResponse
    {
        public Guid ID { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public bool AllowSendingMessages => Rules.Contains((int)ChatGroupRuleList.AllowSendingMessages);
        public bool AllowMakingCalls => Rules.Contains((int)ChatGroupRuleList.AllowMakingCalls);
        public bool AllowEditingDescription => Rules.Contains((int)ChatGroupRuleList.AllowEditingDescription);
        public IEnumerable<ChatGroupParticipantResponse> Participants { get; set; }
        public IEnumerable<int> Rules { get; set; } = Enumerable.Empty<int>();
    }
}
