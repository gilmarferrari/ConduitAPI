namespace Conduit.Application.Queries.Responses
{
    public class UserResponse
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string LastSeen { get; set; }
        public string[] Roles { get; set; }
    }
}
