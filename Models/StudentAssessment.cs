using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class StudentAssessment
    {
        [Key]
        public int AssessmentId { get; set; }

        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        public int TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public virtual Staff Teacher { get; set; }

        public string SubjectName { get; set; }

        public string TaskName { get; set; }

        [Range(0, 10)]
        public int Score { get; set; }

        public string TeacherComments { get; set; }

        public DateTime AssessmentDate { get; set; }
        [Range(1, 4)]
        public int TermNumber { get; set; }

        [Range(1, 12)]
        public int WeekNumber { get; set; }
    }
}