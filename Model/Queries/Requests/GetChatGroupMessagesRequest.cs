using MediatR;
using Model.Queries.Responses;

namespace Model.Queries.Requests
{
    public class GetChatGroupMessagesRequest : IRequest<IEnumerable<ChatGroupMessageResponse>>
    {
        public int UserID { get; set; }
        public int ChatGroupID { get; set; }
        public int PaginationIndex { get; set; } = 0;
        public int PaginationSize { get; set; } = 100;
    }
}
