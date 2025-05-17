using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
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
