using System.ComponentModel.DataAnnotations;

namespace Conduit.Domain.Entities
{
    public class ChatGroupRule
    {
        [Key]
        public Guid ChatGroupID { get; set; }
        [Key]
        public int Rule { get; set; }
        public virtual ChatGroup ChatGroup { get; set; }

        public ChatGroupRule(int rule)
        {
            Rule = rule;
        }

        public ChatGroupRule(ChatGroupRuleList rule)
        {
            Rule = (int)rule;
        }
    }

    [Flags]
    public enum ChatGroupRuleList
    {
        AllowSendingMessages = 0,
        AllowMakingCalls = 1,
        AllowEditingDescription = 2,
    }
}
