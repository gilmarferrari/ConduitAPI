using MediatR;
using Model.Commands.Responses;

namespace Model.Commands.Requests.Put
{
    public class ReadMessagesRequest : IRequest<OkResponse>
    {
        public int UserID { get; set; }
        public int ParticipantID { get; set; }
    }
}
