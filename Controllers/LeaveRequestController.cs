using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models;
using SchoolManagmentSystem.Models;
using System;
using System.Linq;

namespace SchoolManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public LeaveRequestController(ApplicationDBContext context)
        {
            _context = context;
        }

        // NEW: Endpoint to fetch available leave days
        [HttpGet("balance/{staffId}/{leaveType}")]
        public IActionResult GetLeaveBalance(int staffId, LeaveType leaveType)
        {
            var balance = _context.LeaveBalances
                .FirstOrDefault(b => b.StaffId == staffId && b.LeaveType == leaveType);

            // If no record exists yet, assume the default 30 days
            if (balance == null)
            {
                return Ok(new { availableDays = 30 });
            }

            int availableDays = balance.TotalAllocatedDays - balance.UsedDays;
            return Ok(new { availableDays = availableDays });
        }

        [HttpPost("submit")]
        public IActionResult SubmitLeave([FromBody] LeaveRequestDto requestDto)
        {
            try
            {
                int staffId = int.Parse(requestDto.StaffId);

                // 1. Calculate how many days they are asking for (inclusive)
                int requestedDays = (requestDto.EndDate.Date - requestDto.StartDate.Date).Days + 1;

                if (requestedDays <= 0)
                {
                    return BadRequest(new { error = "End date must be the same as or after the start date." });
                }

                // 2. Fetch the teacher's current balance from the database
                var currentBalance = _context.LeaveBalances
                    .FirstOrDefault(b => b.StaffId == staffId && b.LeaveType == requestDto.Type);

                // 3. Gatekeeper Check: Do they have a balance record?
                if (currentBalance == null)
                {
                    return BadRequest(new { error = "No leave balance record found for this leave type." });
                }

                // 4. Gatekeeper Check: Do they have enough days left?
                int availableDays = currentBalance.TotalAllocatedDays - currentBalance.UsedDays;

                if (requestedDays > availableDays)
                {
                    return BadRequest(new { error = $"Insufficient balance. You requested {requestedDays} days, but only have {availableDays} days left." });
                }

                // 5. Apply the strict constructor rules 
                var leaveRequest = new LeaveRequest(
                    staffId,
                    requestDto.Type,
                    requestDto.StartDate,
                    requestDto.EndDate,
                    requestDto.Reason
                );

                // 6. Save the new request to the database
                _context.LeaveRequests.Add(leaveRequest);

                // 7. Deduct the used days from the teacher's balance and update the database
                currentBalance.UsedDays += requestedDays;
                _context.LeaveBalances.Update(currentBalance);

                _context.SaveChanges();

                return Ok(new { message = "Leave request submitted successfully!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }
    }

    // Move this class to a new file named LeaveRequestDto.cs in your Models folder 
    // if it causes a duplicate error in Visual Studio.
    public class LeaveRequestDto
    {
        public string StaffId { get; set; } = string.Empty;
        public LeaveType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}