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

    public string ChildName { get; set; } = default!;

   
    public DateTime ChildDob { get; set; }
    public string Gender { get; set; } = default!;

    public string AutismLevel { get; set; } = default!;
    public string AutismType { get; set; } = default!;

    
    public string PdfPath { get; set; } = default!;

    public string Comments { get; set; } = default!;

    public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public string? Status { get; set; }

        public bool? feesPaid { get; set; }

    }
}
