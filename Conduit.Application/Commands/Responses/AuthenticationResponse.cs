namespace Conduit.Application.Commands.Responses
{
    public class AuthenticationResponse
    {
        public Guid UserID { get; set; }
        public string JWT { get; set; }
        public string LongLivedToken { get; set; }
        public string[] Roles { get; set; }
    }
}
