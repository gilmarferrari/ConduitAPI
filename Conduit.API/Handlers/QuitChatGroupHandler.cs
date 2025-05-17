using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
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
