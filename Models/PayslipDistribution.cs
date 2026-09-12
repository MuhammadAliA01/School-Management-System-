namespace SchoolManagmentSystem.Models
{
    public class PayslipDistribution
    {
        public int PayslipDistributionId { get; set; }

        public int PayslipId { get; set; }

        public int StaffId { get; set; }

        public DateTime ScheduledFor { get; set; }

        public DateTime? DistributedAt { get; set; }

        public DistributionStatus Status { get; set; }

        public string? FailureReason { get; set; }
        public string? AccessToken { get; set; }
    }

    public enum DistributionStatus
    {
        Pending,
        Distributed,
        Failed
    }
}
