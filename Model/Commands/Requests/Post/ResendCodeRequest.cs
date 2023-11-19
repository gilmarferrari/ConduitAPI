using MediatR;
using Model.Commands.Responses;
using System.ComponentModel.DataAnnotations;

namespace Model.Commands.Requests.Post
{
    public class ResendCodeRequest : IRequest<OkResponse>
    {
        public int Type { get; set; }
        [Required, MaxLength(200)]
        public string Email { get; set; }
        public string? IpAddress { get; set; }
        public string? Agent { get; set; }
        public string? Origin { get; set; }
    }
}
