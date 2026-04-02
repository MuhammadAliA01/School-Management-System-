using System.ComponentModel.DataAnnotations;

namespace SchoolManagmentSystem.Models
{
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; } // This is the Primary Key
        public string? Name { get; set; }
    }
}
