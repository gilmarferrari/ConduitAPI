using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
{
    public class UpdateChatGroupHandler : IRequestHandler<UpdateChatGroupRequest, OkResponse>
    {
        private readonly IMessagingService _service;

        public UpdateChatGroupHandler(IMessagingService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(UpdateChatGroupRequest request, CancellationToken cancellationToken)
        {
            return _service.UpdateChatGroup(request);
        }
    }
}
