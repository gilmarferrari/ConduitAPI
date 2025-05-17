using Conduit.Application.Queries.Requests;
using Conduit.Application.Services;
using MediatR;
using Conduit.Application.Queries.Responses;

namespace Conduit.API.Handlers
{
    public class GetUserAccountHandler : IRequestHandler<GetUserAccountRequest, UserAccountResponse>
    {
        private readonly IUserAccountService _service;

        public GetUserAccountHandler(IUserAccountService service)
        {
            _service = service;
        }

        public Task<UserAccountResponse> Handle(GetUserAccountRequest request, CancellationToken cancellationToken)
        {
            return _service.GetUserAccount(request);
        }
    }
}
