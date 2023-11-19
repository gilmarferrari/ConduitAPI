using MediatR;
using Model.Commands.Requests.Post;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
{
    public class QuitChatGroupHandler : IRequestHandler<QuitChatGroupRequest, OkResponse>
    {
        private readonly IMessagingService _service;

        public QuitChatGroupHandler(IMessagingService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(QuitChatGroupRequest request, CancellationToken cancellationToken)
        {
            return _service.QuitChatGroup(request);
        }
    }
}
