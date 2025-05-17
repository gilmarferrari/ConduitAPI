namespace Conduit.Application.Commands.Responses
{
    public class OkResponse
    {
        public string Message { get; set; }
        public OkResponse() { }
        public OkResponse(string message)
        {
            Message = message;
        }
    }
}
