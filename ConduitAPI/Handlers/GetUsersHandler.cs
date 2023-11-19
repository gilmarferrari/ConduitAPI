using MediatR;
using Model.Queries.Requests;
using Model.Queries.Responses;
using Services;

namespace ConduitAPI.Handlers
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
