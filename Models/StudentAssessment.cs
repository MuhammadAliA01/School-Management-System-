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

        // Crucial for special needs: Context on WHY they got that score
        public string TeacherComments { get; set; }

        // When the assessment happened (so you can calculate the monthly average)
        public DateTime AssessmentDate { get; set; }
    }
}