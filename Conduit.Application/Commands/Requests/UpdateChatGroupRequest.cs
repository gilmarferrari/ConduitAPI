using MediatR;
using Conduit.Application.Commands.Responses;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Conduit.Application.Commands.Requests
{
    public class UpdateChatGroupRequest : IRequest<OkResponse>
    {
        [JsonIgnore]
        public Guid LoggedUserID { get; set; }
        public Guid ChatGroupID { get; set; }
        [Required, MinLength(1), MaxLength(100)]
        public string Description { get; set; }
    }
}
