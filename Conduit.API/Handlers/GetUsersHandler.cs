using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Queries.Requests;
using Conduit.Application.Queries.Responses;

namespace Conduit.API.Handlers
{
    public class GetUsersHandler : IRequestHandler<GetUsersRequest, IEnumerable<UserResponse>>
    {
        private readonly IUserService _service;

        public GetUsersHandler(IUserService service)
        {
            _service = service;
        }

        public Task<IEnumerable<UserResponse>> Handle(GetUsersRequest request, CancellationToken cancellationToken)
        {
            return _service.GetUsers(request);
        }
    }
}
