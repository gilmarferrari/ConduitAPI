using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class UserAccount
    {
        public int ID { get; set; }
        [Required, MaxLength(200)]
        public string Email { get; set; }
        [Required, MinLength(8), MaxLength(100)]
        public string Password { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;
        public bool IsAccountActivated { get; set; } = false;
        [MaxLength(8)]
        public string? ActivationToken { get; set; }
        public DateTime? ActivationTokenExpiresAt { get; set; }
        [MaxLength(8)]
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiresAt { get; set; }
    }
}
