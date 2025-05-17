using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
{
    public class DeleteUserAccountHandler : IRequestHandler<DeleteUserAccountRequest, OkResponse>
    {
        private readonly IUserAccountService _service;

        public DeleteUserAccountHandler(IUserAccountService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(DeleteUserAccountRequest request, CancellationToken cancellationToken)
        {
            return _service.DeleteUserAccount(request);
        }
    }
}
