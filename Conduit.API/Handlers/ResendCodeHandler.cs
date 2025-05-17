using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
{
    public class ResendCodeHandler : IRequestHandler<ResendCodeRequest, OkResponse>
    {
        private readonly IUserService _service;

        public ResendCodeHandler(IUserService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(ResendCodeRequest request, CancellationToken cancellationToken)
        {
            return _service.ResendCode(request);
        }
    }
}
