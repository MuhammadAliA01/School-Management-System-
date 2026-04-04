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
    public virtual Parent Parent { get; set; }
    public string ParentName { get; set; }
    public string ParentEmail { get; set; }
    public string ParentPhone { get; set; }

    public string Street { get; set; }
    public string Suburb { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }

    public string ChildName { get; set; }

   
    public DateTime ChildDob { get; set; }
    public string Gender { get; set; }

    public string AutismLevel { get; set; }
    public string AutismType { get; set; }

    //We save the name of the PDF file here, not the actual file itself
    public string PdfPath { get; set; }

    public string Comments { get; set; }

    public DateTime RegistrationDate { get; set; } = DateTime.Now;

}
}
