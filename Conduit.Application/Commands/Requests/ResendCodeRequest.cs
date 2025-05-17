using MediatR;
using Conduit.Application.Commands.Responses;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Conduit.Application.Commands.Requests
{
    public class ResendCodeRequest : IRequest<OkResponse>
    {
        public int Type { get; set; }
        [Required, MaxLength(200)]
        public string Email { get; set; }
        [JsonIgnore]
        public string? IpAddress { get; set; }
        [JsonIgnore]
        public string? Agent { get; set; }
        [JsonIgnore]
        public string? Origin { get; set; }
    }
}
