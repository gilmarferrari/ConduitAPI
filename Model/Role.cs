using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Role
    {
        public int ID { get; set; }
        [Required, MinLength(4), MaxLength(30)]
        public string Code { get;set; }
        [Required, MaxLength(80)]
        public string Description { get;set; }
        public virtual IEnumerable<User> Users { get; set; }
    }
}
