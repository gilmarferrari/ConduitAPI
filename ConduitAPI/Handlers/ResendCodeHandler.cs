using MediatR;
using Model.Commands.Requests.Post;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
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
