using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class RefreshToken
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        [Required, MaxLength(200)]
        public string Token { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => RevokedAt == null && !IsExpired;
        public virtual User User { get; set; }
    }
}
