using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenRequest, AuthenticationResponse>
    {
        private readonly IUserService _service;

        public RefreshTokenHandler(IUserService service)
        {
            _service = service;
        }

        public Task<AuthenticationResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            return _service.RefreshToken(request);
        }
    }
}
