using MediatR;
using Model.Commands.Requests.Put;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
{
    public class ReadMessagesHandler : IRequestHandler<ReadMessagesRequest, OkResponse>
    {
        private readonly IMessagingService _service;

        public ReadMessagesHandler(IMessagingService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(ReadMessagesRequest request, CancellationToken cancellationToken)
        {
            return _service.ReadMessages(request);
        }
    }
}
