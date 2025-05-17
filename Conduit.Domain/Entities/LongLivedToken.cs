using System.ComponentModel.DataAnnotations;

namespace Conduit.Domain.Entities
{
    public class LongLivedToken
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        [Required, MaxLength(200)]
        public string Token { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);
        public DateTime? RevokedAt { get; set; }
        public Guid UserID { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => RevokedAt == null && !IsExpired;
        public virtual User User { get; set; }
    }
}
