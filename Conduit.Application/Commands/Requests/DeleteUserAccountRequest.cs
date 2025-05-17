using MediatR;
using Conduit.Application.Commands.Responses;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Application.Commands.Requests
{
    public class DeleteUserAccountRequest : IRequest<OkResponse>
    {
        [Required]
        public string Email { get; set; }
    }
}
