using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
{
    public class SendMessageHandler : IRequestHandler<SendMessageRequest, OkResponse>
    {
        private readonly IMessagingService _service;

        public SendMessageHandler(IMessagingService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(SendMessageRequest request, CancellationToken cancellationToken)
        {
            return _service.SendMessage(request);
        }
    }
}
