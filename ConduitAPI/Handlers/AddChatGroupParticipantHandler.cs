using MediatR;
using Model.Commands.Requests.Post;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
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
