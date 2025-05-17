using MediatR;
using Conduit.Application.Commands.Responses;

namespace Conduit.Application.Commands.Requests
{
    public class UpdateUserRequest : IRequest<OkResponse>
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<int> Roles { get; set; }
    }
}
