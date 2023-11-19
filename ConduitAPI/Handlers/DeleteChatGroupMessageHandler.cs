using MediatR;
using Model.Commands.Requests.Delete;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
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
