using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Domain.Entities
{
    public class ChatGroupParticipant
    {
        [Key]
        public Guid ChatGroupID { get; set; }
        [Key]
        public Guid ParticipantID { get; set; }
        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        [Required, DefaultValue(false)]
        public bool IsAdmin { get; set; }
        public bool IsActive => EndDate == null;
        public virtual ChatGroup ChatGroup { get; set; }
        public virtual User Participant { get; set; }

        public ChatGroupParticipant(Guid chatGroupID, Guid participantID, bool isAdmin)
        {
            ChatGroupID = chatGroupID;
            ParticipantID = participantID;
            IsAdmin = isAdmin;
        }
    }
}
