using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagmentSystem.Controllers.MobileControllers
{
    public class ClockInRequest
    {
        public int StaffId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]

    public class StaffAttendanceController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public StaffAttendanceController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost("ClockIn")]
        public async Task<IActionResult> ClockIn([FromBody] ClockInRequest request)
        {
            var staff = await _context.Staffs.FindAsync(request.StaffId);
            if (staff == null)
            {
                return NotFound("Staff not found.");
            }
            var attendance = new StaffAttendance
            {
                StaffId = request.StaffId,
                ClockInTime = DateTime.UtcNow,
                ClockInLatitude = request.Latitude,
                ClockInLongitude = request.Longitude
            };
            _context.StaffAttendances.Add(attendance);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Clock-in successful.", attendanceId = attendance.Id });
        }

        [HttpPost("ClockOut")]
        public async Task<IActionResult> ClockOut([FromBody] ClockInRequest request)
        {
            var staff = await _context.Staffs.FindAsync(request.StaffId);
            if (staff == null)
            {
                return NotFound("Staff not found.");
            }
            var attendance = await _context.StaffAttendances
                .Where(a => a.StaffId == request.StaffId && a.ClockOutTime == null)
                .OrderByDescending(a => a.ClockInTime)
                .FirstOrDefaultAsync();
            if (attendance == null)
            {
                return BadRequest("No clock-in record found for this staff.");
            }
            attendance.ClockOutTime = DateTime.UtcNow;
            attendance.ClockOutLatitude = request.Latitude;
            attendance.ClockOutLongitude = request.Longitude;
            // Calculate total hours worked
            if (attendance.ClockInTime.HasValue && attendance.ClockOutTime.HasValue)
            {
                var totalHours = (attendance.ClockOutTime.Value - attendance.ClockInTime.Value).TotalHours;
                attendance.TotalHoursWorked = (decimal)totalHours;
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Clock-out successful.", totalHoursWorked = attendance.TotalHoursWorked });
        }
    }
}