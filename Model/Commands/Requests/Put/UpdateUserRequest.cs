using MediatR;
using Model.Commands.Responses;

namespace Model.Commands.Requests.Put
{
    public class UpdateUserRequest : IRequest<OkResponse>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<int> RolesIDs { get; set; }
    }
}
