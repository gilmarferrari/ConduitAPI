using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
{
    public class AuthenticateHandler : IRequestHandler<AuthenticateRequest, AuthenticationResponse>
    {
        private readonly IUserService _service;

        public AuthenticateHandler(IUserService service)
        {
            _service = service;
        }

        public Task<AuthenticationResponse> Handle(AuthenticateRequest request, CancellationToken cancellationToken)
        {
            return _service.Authenticate(request);
        }
    }
}
