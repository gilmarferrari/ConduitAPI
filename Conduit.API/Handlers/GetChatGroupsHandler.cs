using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Queries.Requests;
using Conduit.Application.Queries.Responses;

namespace Conduit.API.Handlers
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
