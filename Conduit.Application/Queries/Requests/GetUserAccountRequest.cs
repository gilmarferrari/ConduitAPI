using MediatR;
using Conduit.Application.Queries.Responses;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Application.Queries.Requests
{
    public class GetUserAccountRequest : IRequest<UserAccountResponse>
    {
        [Required]
        public string Email { get; set; }
    }
}
