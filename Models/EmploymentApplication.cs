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

        public string? ContractLocation { get; set; }
        public string? IdCopyLocation { get; set; }
        public string? SACELocation { get; set; }
        public string? PoliceClearanceLocation { get; set; }
        public string? BankLetterLocation { get; set; }

        [Display(Name = "Position Applying for")]
        public string selectedRole { get; set; }

        public string? referenceNumber { get; set; }

        public string? Status { get; set; }

        public int? SuitabilityScore { get; set; }

        public string? AIReviewSummary { get; set; }

        public DateTime? dateSent { get; set; }

        public DateTime? interviewDate { get; set; }
        public string? interviewType { get; set; }
        public string? meetingLink { get; set; }

        public string? notes { get; set; }

        public string? MCQ1 { get; set; }
        public string? MCQ2 { get; set; }

        public string? MCQ3{ get; set; }

        public string? MCQ4 { get; set; }

        public string? OEQ1 { get; set; }
        public string? OEQ2 { get; set; }

        public string? OEQ3 { get; set; }






    }
}
