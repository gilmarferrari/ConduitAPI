using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Queries.Requests;
using Conduit.Application.Queries.Responses;

namespace Conduit.API.Handlers
{
    public class GetChatGroupByIDHandler : IRequestHandler<GetChatGroupByIDRequest, ChatGroupResponse>
    {
        private readonly IMessagingService _service;

        public GetChatGroupByIDHandler(IMessagingService service)
        {
            _service = service;
        }

        public Task<ChatGroupResponse> Handle(GetChatGroupByIDRequest request, CancellationToken cancellationToken)
        {
            return _service.GetChatGroupByID(request);
        }
    }
}
