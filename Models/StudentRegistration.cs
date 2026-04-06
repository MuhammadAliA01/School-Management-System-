using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SchoolManagmentSystem.Models
   
{
    public class StudentRegistration
    {

    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int ParentId { get; set; }
    [ForeignKey("ParentId")]
    public virtual Parent Parent { get; set; } = default!;
        public string ParentName { get; set; } = default!;
        public string ParentEmail { get; set; } = default!;
        public string ParentPhone { get; set; } = default!;

        public string Street { get; set; } = default!;
    public string Suburb { get; set; } = default!;
    public string City { get; set; } = default!;
    public string PostalCode { get; set; } = default!;

        [Required(ErrorMessage = "Child's name is required")]

        public string ChildName { get; set; }

        [Required(ErrorMessage = "Child's DOB is required")]


        public DateTime ChildDob { get; set; }

    public string Gender { get; set; }

        [Required(ErrorMessage = "Required")]
        public string AutismLevel { get; set; }

        [Required(ErrorMessage = "Required")]
        public string AutismType { get; set; }

    //We save the name of the PDF file here, not the actual file itself
    public string PdfPath { get; set; }

    public string Comments { get; set; } = default!;

    public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public string? Status { get; set; }

        public bool? feesPaid { get; set; }
        public string? StripeSessionId { get; set; }

    }
}
