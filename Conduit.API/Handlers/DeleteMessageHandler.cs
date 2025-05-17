using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
{
    public class DeleteMessageHandler : IRequestHandler<DeleteMessageRequest, OkResponse>
    {
        private readonly IMessagingService _service;

        public DeleteMessageHandler(IMessagingService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(DeleteMessageRequest request, CancellationToken cancellationToken)
        {
            return _service.DeleteMessage(request);
        }
    }
}
