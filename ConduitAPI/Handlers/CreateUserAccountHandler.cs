using MediatR;
using Model.Commands.Requests.Post;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
{
    public class CreateUserAccountHandler : IRequestHandler<CreateUserAccountRequest, OkResponse>
    {
        private readonly IUserService _service;

        public CreateUserAccountHandler(IUserService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(CreateUserAccountRequest request, CancellationToken cancellationToken)
        {
            return _service.CreateUserAccount(request);
        }
    }
}
