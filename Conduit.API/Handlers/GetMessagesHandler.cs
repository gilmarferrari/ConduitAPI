using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Queries.Requests;
using Conduit.Application.Queries.Responses;

namespace Conduit.API.Handlers
{
    public class GetMessagesHandler : IRequestHandler<GetMessagesRequest, IEnumerable<MessageResponse>>
    {
        private readonly IMessagingService _service;

        public GetMessagesHandler(IMessagingService service)
        {
            _service = service;
        }

        public Task<IEnumerable<MessageResponse>> Handle(GetMessagesRequest request, CancellationToken cancellationToken)
        {
            return _service.GetMessages(request);
        }
    }
}
