using MediatR;
using Conduit.Application.Commands.Responses;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Conduit.Application.Commands.Requests
{
    public class SendChatGroupMessageRequest : IRequest<OkResponse>
    {
        [Required, MinLength(1), MaxLength(1000)]
        public string Content { get; set; }
        [JsonIgnore]
        public Guid SenderID { get; set; }
        public Guid ChatGroupID { get; set; }
        public int? SourceMessageID { get; set; }
    }
}
