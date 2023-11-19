using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class ChatGroup
    {
        public int ID { get; set; }
        [Required, MinLength(1), MaxLength(100)]
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public ICollection<User> Participants { get; set; }
    }
}
