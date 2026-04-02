using System.ComponentModel.DataAnnotations;

namespace SchoolManagmentSystem.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; } // This is the Primary Key

        [Required]
        public string? FullName { get; set; }
        public int TeacherId { get; set; } // Links student to a teacher
    }
}
