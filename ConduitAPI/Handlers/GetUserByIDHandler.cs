using MediatR;
using Model.Queries.Requests;
using Model.Queries.Responses;
using Services;

namespace ConduitAPI.Handlers
{
    public class GetUserByIDHandler : IRequestHandler<GetUserByIDRequest, UserResponse>
    {
        private readonly IUserService _service;

        public GetUserByIDHandler(IUserService service)
        {
            _service = service;
        }

        public Task<UserResponse> Handle(GetUserByIDRequest request, CancellationToken cancellationToken)
        {
            return _service.GetUserByID(request);
        }
    }
}
