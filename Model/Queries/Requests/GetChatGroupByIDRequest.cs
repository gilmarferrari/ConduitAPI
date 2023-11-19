using MediatR;
using Model.Queries.Responses;

namespace Model.Queries.Requests
{
    public class GetChatGroupByIDRequest : IRequest<ChatGroupResponse>
    {
        public int UserID { get; set; }
        public int ChatGroupID { get; set; }
    }
}
