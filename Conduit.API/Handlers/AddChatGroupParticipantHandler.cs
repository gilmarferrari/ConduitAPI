using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
{
    public class AddChatGroupParticipantHandler : IRequestHandler<AddChatGroupParticipantRequest, OkResponse>
    {
        private readonly IMessagingService _service;

        public AddChatGroupParticipantHandler(IMessagingService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(AddChatGroupParticipantRequest request, CancellationToken cancellationToken)
        {
            return _service.AddChatGroupParticipant(request);
        }
    }
}
