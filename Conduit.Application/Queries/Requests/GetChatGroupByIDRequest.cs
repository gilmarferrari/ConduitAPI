using MediatR;
using Conduit.Application.Queries.Responses;
using System.Text.Json.Serialization;

namespace Conduit.Application.Queries.Requests
{
    public class GetChatGroupByIDRequest : IRequest<ChatGroupResponse>
    {
        [JsonIgnore]
        public Guid LoggedUserID { get; set; }
        public Guid ChatGroupID { get; set; }
    }
}
