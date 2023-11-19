using MediatR;
using Model.Queries.Requests;
using Model.Queries.Responses;
using Services;

namespace ConduitAPI.Handlers
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
