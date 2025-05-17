using MediatR;
using Conduit.Application.Queries.Responses;

namespace Conduit.Application.Queries.Requests
{
    public class GetChatGroupMessagesRequest : IRequest<IEnumerable<ChatGroupMessageResponse>>
    {
        public Guid LoggedUserID { get; set; }
        public Guid ChatGroupID { get; set; }
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 100;
    }
}
