using System;

namespace SchoolManagmentSystem.Models
{
    public class LeaveRequest
    {
        public int Id { get; private set; }
        public int StaffId { get; private set; }
        public LeaveType Type { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string Reason { get; private set; }
        public LeaveRequestStatus Status { get; private set; }

        // Fixes the CS8618 warning from your error list
        private LeaveRequest()
        {
            Reason = string.Empty;
        }

        public LeaveRequest(int staffId, LeaveType type, DateTime startDate, DateTime endDate, string reason)
        {
            if (startDate < DateTime.Now.Date)
                throw new ArgumentException("Leave cannot be requested for dates in the past.");

            if (endDate < startDate)
                throw new ArgumentException("End date must be after start date.");

            StaffId = staffId;
            Type = type;
            StartDate = startDate;
            EndDate = endDate;
            Reason = reason;

            // Updated to HOD Review
            Status = LeaveRequestStatus.PendingHODReview;
        }

        public void Approve()
        {
            // Updated to check for HOD status
            if (Status != LeaveRequestStatus.PendingHODReview)
                throw new InvalidOperationException("Only pending requests can be approved.");

            Status = LeaveRequestStatus.Approved;
        }

        public void Reject()
        {
            // Updated to check for HOD status
            if (Status != LeaveRequestStatus.PendingHODReview)
                throw new InvalidOperationException("Only pending requests can be rejected.");

            Status = LeaveRequestStatus.Rejected;
        }
    }
} 