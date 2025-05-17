using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
{
    public class ActivateAccountHandler : IRequestHandler<ActivateAccountRequest, OkResponse>
    {
        private readonly IUserAccountService _service;

        public ActivateAccountHandler(IUserAccountService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(ActivateAccountRequest request, CancellationToken cancellationToken)
        {
            return _service.ActivateAccount(request);
        }
    }
}
