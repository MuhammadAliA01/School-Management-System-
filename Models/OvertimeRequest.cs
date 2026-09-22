using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class OvertimeRequest
    {
        [Key]
        public int OvertimeId { get; set; }

        [Required]
        public int StaffId { get; set; }

        [Required]
        public DateTime DateOfOvertime { get; set; }

        [Required]
        public double HoursRequested { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public DateTime DateSubmitted { get; set; } = DateTime.Now;

        [ForeignKey("StaffId")]
        public virtual Staff? Staff { get; set; }
    }
}
