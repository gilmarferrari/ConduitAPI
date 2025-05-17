using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
{
    public class RevokeTokenHandler : IRequestHandler<RevokeTokenRequest, OkResponse>
    {
        private readonly IUserService _service;

        public RevokeTokenHandler(IUserService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(RevokeTokenRequest request, CancellationToken cancellationToken)
        {
            return _service.RevokeToken(request);
        }
    }
}
