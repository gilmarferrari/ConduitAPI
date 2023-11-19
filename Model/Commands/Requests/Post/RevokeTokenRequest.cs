using MediatR;
using Model.Commands.Responses;
using System.ComponentModel.DataAnnotations;

namespace Model.Commands.Requests.Post
{
    public class RevokeTokenRequest : IRequest<OkResponse>
    {
        [Required]
        public string RefreshToken { get; set; }
        public string? IpAddress { get; set; }
        public string? Agent { get; set; }
        public string? Origin { get; set; }
    }
}
