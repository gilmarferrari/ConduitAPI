using MediatR;
using Model.Commands.Requests.Delete;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
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
