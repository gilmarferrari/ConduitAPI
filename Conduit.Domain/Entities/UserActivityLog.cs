using System.ComponentModel.DataAnnotations;

namespace Conduit.Domain.Entities
{
    public class UserActivityLog
    {
        public int ID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required, MaxLength(400)]
        public string Content { get; set; }
        [MaxLength(50)]
        public string? IpAddress { get; set; }
        public string? Agent { get; set; }
        public string? Origin { get; set; }
        public Guid UserID { get; set; }
        public virtual User User { get; set; }
    }
}
