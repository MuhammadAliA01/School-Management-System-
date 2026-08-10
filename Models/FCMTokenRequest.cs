namespace SchoolManagmentSystem.Models
{
    public class FCMTokenRequest
    {
        public int UserId { get; set; }
        public string Token { get; set; }

        public string Role { get; set; }
    }
}
