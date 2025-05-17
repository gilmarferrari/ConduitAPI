using System.ComponentModel.DataAnnotations;

namespace Conduit.Domain.Entities
{
    public class User
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        [Required, MaxLength(120)]
        public string Name { get; set; }
        public DateTime? LastSeen { get; set; }
        public Guid? UserAccountID { get; set; }
        public virtual UserAccount UserAccount { get; set; }
        public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
        public List<LongLivedToken> LongLivedTokens { get; set; }
        public IEnumerable<ChatGroup> ChatGroups { get; set; }
        public bool OwnsToken(string token) => LongLivedTokens?.Find(x => x.Token == token) != null;
        public bool IsAdmin => Roles.Any(x => x.Role == (int)RoleList.SystemAdmin);
    }
}
