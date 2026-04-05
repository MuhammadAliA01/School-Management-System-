using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    [Index(nameof(Email), IsUnique = true)]

    public class EmploymentApplication
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "CV")]
        public string? CVLocation { get; set; }

        [Display(Name = "Position Applying for")]
        public string selectedRole { get; set; }

        public string? referenceNumber { get; set; }

        public string? Status { get; set; }

        public int? SuitabilityScore { get; set; }

        public string? AIReviewSummary { get; set; }


    }
}
