using System.ComponentModel.DataAnnotations;

namespace SchoolManagmentSystem.Models
{
    public class SarsReport
    {
        [Key]
        public int SarsReportId { get; set; }

        [Required]
        public DateTime PayCycle { get; set; }

        [Required]
        public DateTime GeneratedAt { get; set; }

        [Required]
        public byte[] PdfContent { get; set; } = Array.Empty<byte>();
    }
}
