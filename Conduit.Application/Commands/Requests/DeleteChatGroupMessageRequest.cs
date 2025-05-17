using MediatR;
using Conduit.Application.Commands.Responses;
using System.Text.Json.Serialization;

namespace Conduit.Application.Commands.Requests
{
    public class DeleteChatGroupMessageRequest : IRequest<OkResponse>
    {
        [JsonIgnore]
        public Guid LoggedUserID { get; set; }
        public int MessageID { get; set; }
    }
}
