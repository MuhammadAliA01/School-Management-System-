using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class Parent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("ID Number")]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "ID Number must be exactly 13 digits and contain only numbers")]
        [Required(ErrorMessage = "ID Number is required.")]
        public string? IdNumber { get; set; }

        [DisplayName("First Name")]
        [Required(ErrorMessage = "First Name is required.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [DisplayName("Last Name")]
        public string? Surname { get; set; }

        [DisplayName("Username")]
        [Required(ErrorMessage = "Username is required.")]
        public string? Username { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }

        public string? Salt { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

        public string? FCMToken { get; set; }
    }
}
