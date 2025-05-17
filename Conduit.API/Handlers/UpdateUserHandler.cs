using Conduit.Application.Commands.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.API.Handlers
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
