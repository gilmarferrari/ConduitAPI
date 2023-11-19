using MediatR;
using Model.Commands.Responses;
using System.ComponentModel.DataAnnotations;

namespace Model.Commands.Requests.Post
{
    public class CreateChatGroupRequest : IRequest<OkResponse>
    {
        [Required, MinLength(1), MaxLength(100)]
        public string Description { get; set; }
        public int UserID { get; set; }
    }
}
