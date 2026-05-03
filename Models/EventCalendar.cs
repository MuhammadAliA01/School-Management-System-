using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
namespace SchoolManagmentSystem.Models
{
    public class EventCalendar
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName(" Event ID")]
        public int Id { get; set; }
        [DisplayName("Title")]
        public string Title { get; set; }
        [DisplayName(" Event Description")]
        public string Desc { get; set; }
        [DisplayName("Start Time")]
        public DateTime StartTime { get; set; }
        [DisplayName("End Time")]
        public DateTime EndTime { get; set; }
        public int AttendeeCount { get; set; } // Tracks RSVPs
    }
}
