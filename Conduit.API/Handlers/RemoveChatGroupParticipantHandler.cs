using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
{
    public class RemoveChatGroupParticipantHandler : IRequestHandler<RemoveChatGroupParticipantRequest, OkResponse>
    {
        private readonly IMessagingService _service;

        public RemoveChatGroupParticipantHandler(IMessagingService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(RemoveChatGroupParticipantRequest request, CancellationToken cancellationToken)
        {
            return _service.RemoveChatGroupParticipant(request);
        }
    }
}
