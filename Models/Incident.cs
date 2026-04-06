using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class Incident
    {
        [Key]
        public int IncidentId { get; set; }

        [Required(ErrorMessage = "Please select a student.")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Please select an assigned teacher.")]
        public int TeacherId { get; set; }

        public string? TeacherName { get; set; }

        
        public string? StudentName { get; set; }

        [Required(ErrorMessage = "Please select the date.")]
        [DataType(DataType.Date)] // Changed from DateTime to Date
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] // Removed HH:mm
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Please provide a description of the incident.")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 1000 characters.")]
        public string? Description { get; set; }

        [Display(Name = "Action Taken")]
        public string? ActionTaken { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        [ForeignKey("TeacherId")]
        public virtual Teacher? Teacher { get; set; }
    }
}

