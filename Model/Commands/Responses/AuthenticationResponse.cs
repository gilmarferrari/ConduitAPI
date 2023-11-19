namespace Model.Commands.Responses
{
    public class AuthenticationResponse
    {
        public int UserID { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string[] Roles { get; set; }
    }
}
