using MediatR;
using Model.Commands.Requests.Post;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
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
