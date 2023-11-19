using MediatR;
using Model.Commands.Requests.Put;
using Model.Commands.Responses;
using Services;

namespace ConduitAPI.Handlers
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserRequest, OkResponse>
    {
        private readonly IUserService _service;

        public UpdateUserHandler(IUserService service)
        {
            _service = service;
        }

        public Task<OkResponse> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            return _service.UpdateUser(request);
        }
    }
}
