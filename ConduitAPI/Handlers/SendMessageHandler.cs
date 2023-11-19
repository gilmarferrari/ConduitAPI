using MediatR;
using Model.Commands.Requests.Post;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
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
