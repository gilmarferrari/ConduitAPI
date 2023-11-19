using MediatR;
using Model.Commands.Requests.Post;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
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
