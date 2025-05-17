using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
{
    public class CreateUserAccountHandler : IRequestHandler<CreateUserAccountRequest, OkResponse>
    {
        private readonly IUserAccountService _service;

        public CreateUserAccountHandler(IUserAccountService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(CreateUserAccountRequest request, CancellationToken cancellationToken)
        {
            return _service.CreateUserAccount(request);
        }
    }
}
