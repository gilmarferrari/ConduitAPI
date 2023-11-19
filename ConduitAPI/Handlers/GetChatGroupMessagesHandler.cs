using MediatR;
using Model.Queries.Requests;
using Model.Queries.Responses;
using Services;

namespace ConduitAPI.Handlers
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
