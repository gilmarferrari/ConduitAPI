using MediatR;
using Model.Commands.Responses;
using System.ComponentModel.DataAnnotations;

namespace Model.Commands.Requests.Put
{
    public class UpdateChatGroupRequest : IRequest<OkResponse>
    {
        public int UserID { get; set; }
        public int ChatGroupID { get; set; }
        [Required, MinLength(1), MaxLength(100)]
        public string Description { get; set; }
    }
}
