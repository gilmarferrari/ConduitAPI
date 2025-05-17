using MediatR;
using Conduit.Application.Commands.Responses;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Conduit.Application.Commands.Requests
{
    public class ResetPasswordRequest : IRequest<OkResponse>
    {
        [Required]
        public string Email { get; set; }
        [Required, MaxLength(8)]
        public string ResetToken { get; set; }
        [Required, MinLength(8), MaxLength(100)]
        public string Password { get; set; }
        [Required, MinLength(8), MaxLength(100), Compare("Password")]
        public string ConfirmPassword { get; set; }
        [JsonIgnore]
        public string? IpAddress { get; set; }
        [JsonIgnore]
        public string? Agent { get; set; }
        [JsonIgnore]
        public string? Origin { get; set; }
    }
}
