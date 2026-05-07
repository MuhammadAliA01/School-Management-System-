using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class Student
    {
        

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }
        public int ParentId { get; set; }
        [ForeignKey("ParentId")]
        public virtual Parent Parent { get; set; }
        public string ParentName { get; set; }
        public string ParentEmail { get; set; }
        public string ParentPhone { get; set; }


        public int TeacherId { get; set; } //required for Aamir's part. but in reality students arent linked to specific teachers

        public string Street { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public string? FullName { get; set; }


        public DateTime ChildDob { get; set; }
        public string Gender { get; set; }

        public string AutismLevel { get; set; }
        public string AutismType { get; set; }

        //connects your Student to their Files in a way that the database understands.
        public virtual ICollection<StudentDocument> Documents { get; set; } = new List<StudentDocument>();    //NOTE: I HAVEN'T UPDATED TO DATABASE

        public string Comments { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public string? AiTeachingPlan { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalFees { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }

        public decimal BalanceDue
        {
            get { return TotalFees - AmountPaid; }
        }

        public int? ClassId { get; set; }

        [ForeignKey("ClassId")]
        public virtual AssignedClassStudent AssignedClassStudents { get; set; }

    }
}
