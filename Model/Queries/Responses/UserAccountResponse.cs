namespace Model.Queries.Responses
{
    public class UserAccountResponse
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsAccountActivated { get; set; }
    }
}
