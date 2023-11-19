using MediatR;
using Model.Commands.Requests.Post;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
{
    public class ActivateAccountHandler : IRequestHandler<ActivateAccountRequest, OkResponse>
    {
        private readonly IUserService _service;

        public ActivateAccountHandler(IUserService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(ActivateAccountRequest request, CancellationToken cancellationToken)
        {
            return _service.ActivateAccount(request);
        }
    }
}
