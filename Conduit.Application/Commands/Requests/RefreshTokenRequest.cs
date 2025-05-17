using MediatR;
using Conduit.Application.Commands.Responses;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Conduit.Application.Commands.Requests
{
    public class RefreshTokenRequest : IRequest<AuthenticationResponse>
    {
        [Required]
        public string LongLivedToken { get; set; }
        [JsonIgnore]
        public string? IpAddress { get; set; }
        [JsonIgnore]
        public string? Agent { get; set; }
        [JsonIgnore]
        public string? Origin { get; set; }
    }
}
