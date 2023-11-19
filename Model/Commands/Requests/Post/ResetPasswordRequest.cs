
using MediatR;
using Model.Commands.Responses;
using System.ComponentModel.DataAnnotations;

namespace Model.Commands.Requests.Post
{
    public class ResetPasswordRequest : IRequest<OkResponse>
    {
        [Required]
        public string ResetToken { get; set; }
        [Required, MinLength(8), MaxLength(100)]
        public string Password { get; set; }
        [Required, MinLength(8), MaxLength(100), Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string? IpAddress { get; set; }
        public string? Agent { get; set; }
        public string? Origin { get; set; }
    }
}
