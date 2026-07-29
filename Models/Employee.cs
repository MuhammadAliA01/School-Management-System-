using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagmentSystem.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [DisplayName("Employee Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Position { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Date Joined")]
        public DateTime DateJoined { get; set; } = DateTime.Now;

        public string? DocumentPath { get; set; } // Optional to allow manual entry
    }
}