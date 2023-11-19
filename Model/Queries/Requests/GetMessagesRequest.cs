using MediatR;
using Model.Queries.Responses;

namespace Model.Queries.Requests
{
    public class GetMessagesRequest : IRequest<IEnumerable<MessageResponse>>
    {
        public int UserID { get; set; }
        public int ParticipantID { get; set; }
        public int PaginationIndex { get; set; } = 0;
        public int PaginationSize { get; set; } = 100;
    }
}
