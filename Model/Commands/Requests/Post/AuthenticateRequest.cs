using MediatR;
using Model.Commands.Responses;
using System.ComponentModel.DataAnnotations;

namespace Model.Commands.Requests.Post
{
    public class AuthenticateRequest : IRequest<AuthenticationResponse>
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string? IpAddress { get; set; }
        public string? Agent { get; set; }
        public string? Origin { get; set; }
    }
}
