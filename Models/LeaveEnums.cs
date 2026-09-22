namespace SchoolManagementSystem.Models
{
    public enum LeaveRequestStatus
    {
        PendingHODReview, // Updated to match your workflow plan
        Approved,
        Rejected,
        CancelledByTeacher
    }

    public enum LeaveType
    {
        SickLeave,
        FamilyResponsibility,
        StudyLeave,
        AnnualLeave
    }
} 