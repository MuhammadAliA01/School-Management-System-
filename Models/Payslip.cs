namespace SchoolManagmentSystem.Models
{
    public class Payslip
    {
        public int PayslipId { get; set; }
        public int StaffId { get; set; }
        public DateTime PayPeriodStart { get; set; }
        public DateTime PayPeriodEnd { get; set; }
        public byte[] PdfContent { get; set; }
        public PayslipStatus Status { get; set; }
        public string FailureReason { get; set; }
        public int CurrentVersion { get; set; } = 1;
        public DateTime GeneratedAt { get; set; }
    }

    public enum PayslipStatus
    {
        Pending,
        Generated,
        Failed
    }
}
