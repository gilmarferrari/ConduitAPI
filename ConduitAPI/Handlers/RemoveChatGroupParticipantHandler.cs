using MediatR;
using Model.Commands.Requests.Post;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
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
