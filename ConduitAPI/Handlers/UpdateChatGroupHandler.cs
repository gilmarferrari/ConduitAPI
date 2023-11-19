using MediatR;
using Model.Commands.Requests.Put;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
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
