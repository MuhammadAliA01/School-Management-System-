using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class StaffAttendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StaffId { get; set; }
        public Staff Staff { get; set; }

        public DateTime? ClockInTime { get; set; }
        public DateTime? ClockOutTime { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? ClockInLatitude { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? ClockInLongitude { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? ClockOutLatitude { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? ClockOutLongitude { get; set; }

        [MaxLength(255)]
        public string? ClockInPhotoUrl { get; set; }

        [MaxLength(255)]
        public string? ClockOutPhotoUrl { get; set; }

        public bool IsAnomaly { get; set; } = false;

        [MaxLength(255)]
        public string? AnomalyReason { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? TotalHoursWorked { get; set; }
        public byte[]? ClockInPhoto { get; set; }
        public byte[]? ClockOutPhoto { get; set; }
    }
}
