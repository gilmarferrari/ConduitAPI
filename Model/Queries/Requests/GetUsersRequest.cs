using MediatR;
using Model.Queries.Responses;

namespace Model.Queries.Requests
{
    public class GetUsersRequest : IRequest<IEnumerable<UserResponse>>
    {
        public bool ActiveOnly { get; set; } = false;
    }
}
