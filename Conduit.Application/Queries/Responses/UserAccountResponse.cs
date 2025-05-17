namespace Conduit.Application.Queries.Responses
{
    public class UserAccountResponse
    {
        public Guid ID { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsAccountActivated { get; set; }
    }
}
