using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Domain.Entities
{
    public class UserAccount
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        [Required, MaxLength(200), EmailAddress]
        public string Email { get; set; }
        [Required, MinLength(8), MaxLength(100)]
        public string PasswordHash { get; set; }
        [Required, DefaultValue(true)]
        public bool IsActive { get; set; } = true;
        public DateTime? ActivationDate { get; set; }
        [MaxLength(8)]
        public string? ActivationToken { get; set; }
        public DateTime? ActivationTokenExpiresAt { get; set; }
        [MaxLength(8)]
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiresAt { get; set; }
        public bool IsAccountActivated => ActivationDate != null;
    }
}
