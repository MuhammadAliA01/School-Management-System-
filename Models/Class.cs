using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagmentSystem.Models
{
    public class Class
    {
        [Key]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Class Name is required")]
        [StringLength(50)]
        [Display(Name = "Class Name (e.g., Grade 10A)")]
        public string ClassName { get; set; }

        [StringLength(20)]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }

        // Navigation property linking this class to its schedules
        public virtual ICollection<StaffSchedule> StaffSchedules { get; set; }
    }
}
