using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
{
    public class DeleteChatGroupMessageHandler : IRequestHandler<DeleteChatGroupMessageRequest, OkResponse>
    {
        private readonly IMessagingService _service;

        public DeleteChatGroupMessageHandler(IMessagingService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(DeleteChatGroupMessageRequest request, CancellationToken cancellationToken)
        {
            return _service.DeleteChatGroupMessage(request);
        }
    }
}
