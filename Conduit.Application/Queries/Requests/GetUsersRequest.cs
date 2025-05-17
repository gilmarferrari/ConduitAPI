using MediatR;
using Conduit.Application.Queries.Responses;

namespace Conduit.Application.Queries.Requests
{
    public class GetUsersRequest : IRequest<IEnumerable<UserResponse>>
    {
        public bool ActiveOnly { get; set; } = false;
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 100;
    }
}
