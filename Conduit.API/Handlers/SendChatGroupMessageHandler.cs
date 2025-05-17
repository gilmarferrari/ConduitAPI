using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
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
