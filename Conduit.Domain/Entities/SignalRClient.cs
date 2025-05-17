using System.ComponentModel.DataAnnotations;

namespace Conduit.Domain.Entities
{
    public class SignalRClient
    {
        [Key, Required]
        public string ConnectionID { get; set; }
        [Key]
        public Guid UserID { get; set; }
        public virtual User User { get; set; }

        public SignalRClient(string connectionID, Guid userID)
        {
            ConnectionID = connectionID;
            UserID = userID;
        }
    }
}
