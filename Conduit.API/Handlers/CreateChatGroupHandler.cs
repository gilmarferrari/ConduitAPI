using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
{
    public class CreateChatGroupHandler : IRequestHandler<CreateChatGroupRequest, OkResponse>
    {
        private readonly IMessagingService _service;

        public CreateChatGroupHandler(IMessagingService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(CreateChatGroupRequest request, CancellationToken cancellationToken)
        {
            return _service.CreateChatGroup(request);
        }
    }
}
