namespace SchoolManagmentSystem.Models
{
    public class PayslipHistoryItemDto
    {
        public int PayslipId { get; set; }
        public DateTime PayPeriodStart { get; set; }
        public DateTime PayPeriodEnd { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? FailureReason { get; set; }
        public int CurrentVersion { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}