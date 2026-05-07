namespace SchoolManagmentSystem.Models
{
    public class RSVP
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string UserName { get; set; }
        public bool IsAttending { get; set; }
    }
}
