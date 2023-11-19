using MediatR;
using Model.Queries.Requests;
using Model.Queries.Responses;
using Services;

namespace ConduitAPI.Handlers
{
    public class GetChatGroupByIDHandler : IRequestHandler<GetChatGroupByIDRequest, ChatGroupResponse>
    {
        private readonly IMessagingService _service;

        public GetChatGroupByIDHandler(IMessagingService service)
        {
            _service = service;
        }

        public Task<ChatGroupResponse> Handle(GetChatGroupByIDRequest request, CancellationToken cancellationToken)
        {
            return _service.GetChatGroupByID(request);
        }
    }
}
