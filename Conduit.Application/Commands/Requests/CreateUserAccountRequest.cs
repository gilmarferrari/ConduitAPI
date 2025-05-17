using MediatR;
using Conduit.Application.Commands.Responses;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Application.Commands.Requests
{
    public class CreateUserAccountRequest : IRequest<OkResponse>
    {
        [Required, MaxLength(200)]
        public string Email { get; set; }
        [Required, MinLength(8), MaxLength(100)]
        public string Password { get; set; }
    }
}
