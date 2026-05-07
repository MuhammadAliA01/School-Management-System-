using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagmentSystem.Models
{
    public class ConductReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Severity { get; set; }

        [Required]
        public DateTime IncidentDate { get; set; }

        public string InterventionPlan { get; set; }

        public DateTime DateLogged { get; set; } = DateTime.Now;

        public bool IsReviewedByAdmin { get; set; } = false;

        public string? ParentMessage { get; set; } 
        public bool IsViewedByParent { get; set; } 
        public DateTime? DateViewed { get; set; }  
    }
}