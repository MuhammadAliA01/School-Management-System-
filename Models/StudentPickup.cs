using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class StudentPickup
    {
        [Key]
        public int StudentPickupId { get; set; }

        public int StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; }

        public string? DelegatePIN { get; set; }

        public string PickupStatus { get; set; } = "Not Picked Up";

        public DateTime? PickupTime { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}