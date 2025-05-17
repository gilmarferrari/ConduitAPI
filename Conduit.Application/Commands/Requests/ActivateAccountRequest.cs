using MediatR;
using Conduit.Application.Commands.Responses;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Conduit.Application.Commands.Requests
{
    public class ActivateAccountRequest : IRequest<OkResponse>
    {
        [Required]
        public string Email { get; set; }
        [Required, MaxLength(8)]
        public string ActivationToken { get; set; }
        [Required, MaxLength(120)]
        public string Name { get; set; }
        [JsonIgnore]
        public string? IpAddress { get; set; }
        [JsonIgnore]
        public string? Agent { get; set; }
        [JsonIgnore]
        public string? Origin { get; set; }
    }
}
