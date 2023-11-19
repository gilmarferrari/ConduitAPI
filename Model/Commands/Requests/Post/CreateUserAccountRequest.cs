using MediatR;
using Model.Commands.Responses;
using System.ComponentModel.DataAnnotations;

namespace Model.Commands.Requests.Post
{
    public class CreateUserAccountRequest : IRequest<OkResponse>
    {
        [Required, MaxLength(200)]
        public string Email { get; set; }
        [Required, MinLength(8), MaxLength(100)]
        public string Password { get; set; }
    }
}
