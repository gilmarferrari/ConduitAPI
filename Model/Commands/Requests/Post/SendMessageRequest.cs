using MediatR;
using Model.Commands.Responses;
using System.ComponentModel.DataAnnotations;

namespace Model.Commands.Requests.Post
{
    public class SendMessageRequest : IRequest<OkResponse>
    {
        [Required, MinLength(1), MaxLength(1000)]
        public string Content { get; set; }
        public int SenderID { get; set; }
        public int RecipientID { get; set; }
        public int? SourceMessageID { get; set; }
    }
}
