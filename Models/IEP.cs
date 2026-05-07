using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class IEP
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        // The '?' is CRITICAL. It tells the system not to validate 
        // the whole Student object during the Create process.
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        [Required(ErrorMessage = "Academic Goals are required")]
        [Display(Name = "Academic Goals")]
        public string AcademicGoals { get; set; }

        [Required(ErrorMessage = "Behavioural Goals are required")]
        [Display(Name = "Behavioural Goals")]
        public string BehaviouralGoals { get; set; }

        [Required(ErrorMessage = "Developmental Goals are required")]
        [Display(Name = "Developmental Goals")]
        public string DevelopmentalGoals { get; set; }

        [Required(ErrorMessage = "Please specify the Term")]
        [Display(Name = "Time/Term")]
        public string Term { get; set; }

        [Required(ErrorMessage = "Target Date is required")]
        [Display(Name = "Goal Target Date")]
        [DataType(DataType.Date)]
        public DateTime TargetDate { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
