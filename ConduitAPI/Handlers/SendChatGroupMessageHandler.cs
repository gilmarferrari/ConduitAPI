using MediatR;
using Model.Commands.Requests.Post;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
{
    public class SendChatGroupMessageHandler : IRequestHandler<SendChatGroupMessageRequest, OkResponse>
    {
        private readonly IMessagingService _service;

        public SendChatGroupMessageHandler(IMessagingService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(SendChatGroupMessageRequest request, CancellationToken cancellationToken)
        {
            return _service.SendChatGroupMessage(request);
        }
    }
}
