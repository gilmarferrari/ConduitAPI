using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
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
