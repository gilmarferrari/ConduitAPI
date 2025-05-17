using System.ComponentModel.DataAnnotations;

namespace Conduit.Domain.Entities
{
    public class ChatGroup
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        [Required, MinLength(1), MaxLength(100)]
        public string Description { get; set; }
        public Guid CreatorID { get; set; }
        public virtual User Creator { get; set; }
        public ICollection<ChatGroupParticipant> Participants { get; set; } = new List<ChatGroupParticipant>();
        public ICollection<ChatGroupRule> Rules { get; set; } = new List<ChatGroupRule>();
    }
}
