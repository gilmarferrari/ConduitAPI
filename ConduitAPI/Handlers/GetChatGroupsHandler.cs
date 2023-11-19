using MediatR;
using Model.Queries.Requests;
using Model.Queries.Responses;
using Services;

namespace ConduitAPI.Handlers
{
    public class GetChatGroupsHandler : IRequestHandler<GetChatGroupsRequest, IEnumerable<ChatGroupResponse>>
    {
        private readonly IMessagingService _service;

        public GetChatGroupsHandler(IMessagingService service)
        {
            _service = service;
        }

        public Task<IEnumerable<ChatGroupResponse>> Handle(GetChatGroupsRequest request, CancellationToken cancellationToken)
        {
            return _service.GetChatGroups(request);
        }
    }
}
