namespace SchoolManagementSystem.Models
{
    public class LeaveBalance
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public LeaveType LeaveType { get; set; }

        // Automatically grants 30 days to every new record
        public int TotalAllocatedDays { get; set; } = 30;

        // Ensures they start with 0 days used
        public int UsedDays { get; set; } = 0;
    }
}