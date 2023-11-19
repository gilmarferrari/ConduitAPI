using MediatR;
using Model.Commands.Responses;

namespace Model.Commands.Requests.Delete
{
    public class DeleteMessageRequest : IRequest<OkResponse>
    {
        public int UserID { get; set; }
        public int MessageID { get; set; }
    }
}
