using MediatR;
using Conduit.Application.Queries.Responses;

namespace Conduit.Application.Queries.Requests
{
    public class GetUserByIDRequest : IRequest<UserResponse>
    {
        public Guid ID { get; set; }
    }
}
