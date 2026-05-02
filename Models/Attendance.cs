using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }
       
        [Required(ErrorMessage = "A student must be selected.")]
        public int StudentId { get; set; }

        public virtual Student Student { get; set; }

        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Status cannot be blank.")]
        public string Status { get; set; }
    }
}
