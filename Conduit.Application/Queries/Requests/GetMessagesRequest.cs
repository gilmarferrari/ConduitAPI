using MediatR;
using Conduit.Application.Queries.Responses;

namespace Conduit.Application.Queries.Requests
{
    public class GetMessagesRequest : IRequest<IEnumerable<MessageResponse>>
    {
        public Guid LoggedUserID { get; set; }
        public Guid ParticipantID { get; set; }
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 100;
    }
}
