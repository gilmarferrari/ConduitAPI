using MediatR;
using Model.Commands.Responses;

namespace Model.Commands.Requests.Delete
{
    public class DeleteUserAccountRequest : IRequest<OkResponse>
    {
        public int ID { get; set; }
    }
}
