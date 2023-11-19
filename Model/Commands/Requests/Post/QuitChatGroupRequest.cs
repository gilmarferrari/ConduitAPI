using MediatR;
using Model.Commands.Responses;

namespace Model.Commands.Requests.Post
{
    public class QuitChatGroupRequest : IRequest<OkResponse>
    {
        public int UserID { get; set; }
        public int ChatGroupID { get; set; }
    }
}
