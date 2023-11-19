using MediatR;
using Model.Commands.Requests.Post;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
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
