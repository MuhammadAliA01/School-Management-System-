using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class AssignedClassStudent
    {
        [Key]
        public int ClassId { get; set; }

        public string ClassName { get; set; } 

        public string Room { get; set; } 

        public int TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public virtual Staff Teacher { get; set; }

        public int TermNumber { get; set; }
        public int WeekNumber { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}