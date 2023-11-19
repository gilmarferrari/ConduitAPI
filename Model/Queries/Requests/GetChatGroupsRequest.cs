using MediatR;
using Model.Queries.Responses;

namespace Model.Queries.Requests
{
    public class GetChatGroupsRequest : IRequest<IEnumerable<ChatGroupResponse>>
    {
        public int UserID { get; set; }
    }
}
