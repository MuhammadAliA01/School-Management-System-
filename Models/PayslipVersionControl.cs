using System.ComponentModel.DataAnnotations;

namespace SchoolManagmentSystem.Models
{
    public class PayslipVersionControl
    {
        public int PayslipVersionControlId { get; set; }
        public int PayslipId { get; set; }
        public byte[] ArchivedPdfContent { get; set; }
        public int ArchivedVersionNumber { get; set; }
        [Required]
        public string RegenerationReason { get; set; }
        public DateTime ArchivedAt { get; set; }
    }
}
