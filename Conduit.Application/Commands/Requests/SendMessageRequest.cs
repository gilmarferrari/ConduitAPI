using MediatR;
using Conduit.Application.Commands.Responses;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace Conduit.Application.Commands.Requests
{
    public class SendMessageRequest : IRequest<OkResponse>
    {
        [Required, MinLength(1), MaxLength(1000)]
        public string Content { get; set; }
        [JsonIgnore]
        public Guid SenderID { get; set; }
        public Guid RecipientID { get; set; }
        [DefaultValue(null)]
        public int? SourceMessageID { get; set; }
    }
}
