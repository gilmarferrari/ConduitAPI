using MediatR;
using Conduit.Application.Commands.Responses;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Conduit.Application.Commands.Requests
{
    public class CreateChatGroupRequest : IRequest<OkResponse>
    {
        [Required, MinLength(1), MaxLength(100)]
        public string Description { get; set; }
        [Required]
        public bool AllowSendingMessages { get; set; }
        [Required]
        public bool AllowMakingCalls { get; set; }
        [Required]
        public bool AllowEditingDescription { get; set; }
        [JsonIgnore]
        public Guid LoggedUserID { get; set; }
    }
}
