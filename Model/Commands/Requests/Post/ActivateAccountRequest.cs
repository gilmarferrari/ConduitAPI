using MediatR;
using Model.Commands.Responses;
using System.ComponentModel.DataAnnotations;

namespace Model.Commands.Requests.Post
{
    public class ActivateAccountRequest : IRequest<OkResponse>
    {
        [Required]
        public string ActivationToken { get; set; }
        [Required, MaxLength(120)]
        public string Name { get; set; }
        public string? IpAddress { get; set; }
        public string? Agent { get; set; }
        public string? Origin { get; set; }
    }
}
