using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class StaffSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        // Assuming your teacher table is called Employee based on your Solution Explorer,
        // if it is specifically called Teacher, change Employee to Teacher below.
        [Required]
        public int TeacherId { get; set; }

        [Required]
        public int ClassId { get; set; }
        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ScheduleDate { get; set; }

        public string ShiftType { get; set; } = "Full Day";
    }
}