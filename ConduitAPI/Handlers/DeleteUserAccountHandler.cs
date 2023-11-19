using MediatR;
using Model.Commands.Requests.Delete;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
{
    public class DeleteUserAccountHandler : IRequestHandler<DeleteUserAccountRequest, OkResponse>
    {
        private readonly IUserService _service;

        public DeleteUserAccountHandler(IUserService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(DeleteUserAccountRequest request, CancellationToken cancellationToken)
        {
            return _service.DeleteUserAccount(request);
        }
    }
}
