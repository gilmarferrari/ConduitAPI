using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Queries.Requests;
using Conduit.Application.Queries.Responses;

namespace Conduit.API.Handlers
{
    public class GetChatGroupMessagesHandler : IRequestHandler<GetChatGroupMessagesRequest, IEnumerable<ChatGroupMessageResponse>>
    {
        private readonly IMessagingService _service;

        public GetChatGroupMessagesHandler(IMessagingService service)
        {
            _service = service;
        }

        public Task<IEnumerable<ChatGroupMessageResponse>> Handle(GetChatGroupMessagesRequest request, CancellationToken cancellationToken)
        {
            return _service.GetChatGroupMessages(request);
        }
    }
}
