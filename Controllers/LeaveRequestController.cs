using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
using System;
using System.Linq;

namespace SchoolManagmentSystem.Controllers
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
        public IActionResult GetLeaveBalance(
     int staffId,
     LeaveType leaveType)
        {
            var balance = _context.LeaveBalances
                .FirstOrDefault(b =>
                    b.StaffId == staffId &&
                    b.LeaveType == leaveType);

            if (balance == null)
            {
                balance = new LeaveBalance
                {
                    StaffId = staffId,
                    LeaveType = leaveType,
                    TotalAllocatedDays = 30,
                    UsedDays = 0
                };

                _context.LeaveBalances.Add(balance);
                _context.SaveChanges();
            }

            int availableDays =
                balance.TotalAllocatedDays -
                balance.UsedDays;

            return Ok(new
            {
                availableDays = availableDays
            });
        }

        [HttpPost("submit")]
        public IActionResult SubmitLeave([FromBody] LeaveRequestDto requestDto)
        {
            try
            {
                int staffId = int.Parse(requestDto.StaffId);

                int requestedDays =
                    (requestDto.EndDate.Date - requestDto.StartDate.Date).Days + 1;

                if (requestedDays <= 0)
                {
                    return BadRequest(new
                    {
                        error = "End date must be the same as or after the start date."
                    });
                }

                var currentBalance = _context.LeaveBalances
                    .FirstOrDefault(b =>
                        b.StaffId == staffId &&
                        b.LeaveType == requestDto.Type);

                if (currentBalance == null)
                {
                    currentBalance = new LeaveBalance
                    {
                        StaffId = staffId,
                        LeaveType = requestDto.Type,
                        TotalAllocatedDays = 30,
                        UsedDays = 0
                    };

                    _context.LeaveBalances.Add(currentBalance);
                }

                int availableDays =
                    currentBalance.TotalAllocatedDays -
                    currentBalance.UsedDays;

                if (requestedDays > availableDays)
                {
                    return BadRequest(new
                    {
                        error =
                            $"Insufficient balance. You requested " +
                            $"{requestedDays} days, but only " +
                            $"{availableDays} days are available."
                    });
                }

                var leaveRequest = new LeaveRequest(
                    staffId,
                    requestDto.Type,
                    requestDto.StartDate,
                    requestDto.EndDate,
                    requestDto.Reason
                );

                _context.LeaveRequests.Add(leaveRequest);

                _context.SaveChanges();

                return Ok(new
                {
                    message = "Leave request submitted successfully!"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message
                });
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