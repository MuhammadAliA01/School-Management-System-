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

        //connects your Student to their Files in a way that the database understands.
        public virtual ICollection<StudentDocument> Documents { get; set; } = new List<StudentDocument>();  //NOTE: I HAVEN'T UPDATED TO DATABASE

        public string Comments { get; set; } = default!;

    public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public string? Status { get; set; }

        public bool? feesPaid { get; set; }
        public string? StripeSessionId { get; set; }

    }


    public class StudentDocument
    {
        [Key]
        public int Id { get; set; }

        public string FilePath { get; set; } = default!;
        public string FileName { get; set; } = default!;

        public int StudentRegistrationId { get; set; }
        [ForeignKey("StudentRegistrationId")]
        public virtual StudentRegistration StudentRegistration { get; set; } = default!;
    }
}
