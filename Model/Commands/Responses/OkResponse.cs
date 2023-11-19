namespace Model.Commands.Responses
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
