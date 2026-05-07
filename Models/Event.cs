namespace SchoolManagmentSystem.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public DateTime EventDate { get; set; }
        public string Description { get; set; }
        public virtual ICollection<RSVP> RSVPs { get; set; } = new List<RSVP>();
    }
}
