using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models; // Your models namespace
using SchoolManagmentSystem.Models;
using System;

namespace SchoolManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        // Injects the database context so we can save data
        public LeaveRequestController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost("submit")]
        public IActionResult SubmitLeave([FromBody] LeaveRequestDto requestDto)
        {
            try
            {
                // We use your strict constructor to ensure the business rules (like setting it to PendingHODReview) are applied
                var leaveRequest = new LeaveRequest(
                    requestDto.StaffId,
                    requestDto.Type,
                    requestDto.StartDate,
                    requestDto.EndDate,
                    requestDto.Reason
                );

                _context.LeaveRequests.Add(leaveRequest);
                _context.SaveChanges();

                // Sends a success message back to Flutter
                return Ok(new { message = "Leave request submitted successfully!" });
            }
            catch (ArgumentException ex)
            {
                // Catches the date errors we wrote in the constructor (like past dates)
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }
    }

    // This DTO (Data Transfer Object) simply catches the raw JSON from Flutter
    public class LeaveRequestDto
    {
        public int StaffId { get; set; }
        public LeaveType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
} 
