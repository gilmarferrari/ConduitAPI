using MediatR;
using Model.Queries.Responses;

namespace Model.Queries.Requests
{
    public class GetUserByIDRequest : IRequest<UserResponse>
    {
        public int ID { get; set; }
    }
}
