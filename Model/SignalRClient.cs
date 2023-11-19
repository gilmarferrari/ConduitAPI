using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class SignalRClient
    {
        public int ID { get; set; }
        [Required]
        public string ConnectionID { get; set; }
        public int UserID { get;set; }
        public virtual User User { get; set; }
    }
}
