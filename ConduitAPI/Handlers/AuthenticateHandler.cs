using MediatR;
using Model.Commands.Requests.Post;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
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
