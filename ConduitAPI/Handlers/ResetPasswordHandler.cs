using MediatR;
using Model.Commands.Requests.Post;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
{
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordRequest, OkResponse>
    {
        private readonly IUserService _service;

        public ResetPasswordHandler(IUserService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            return _service.ResetPassword(request);
        }
    }
}
