using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Queries.Requests;
using Conduit.Application.Queries.Responses;

namespace Conduit.API.Handlers
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
