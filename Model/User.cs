using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class User
    {
        public int ID { get;set; }
        [Required, MaxLength(120)]
        public string Name { get; set; }
        public DateTime? LastSeen { get;set; }
        public int? UserAccountID { get; set; }
        public virtual UserAccount UserAccount { get; set; }
        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public IEnumerable<ChatGroup> ChatGroups { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
        public bool OwnsToken(string token) => RefreshTokens?.Find(x => x.Token == token) != null;
        public bool IsAdmin => Roles.Any(r => r.Code.Equals("SystemAdmin"));
    }
}
