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
        public string? PhotoBase64 { get; set; }
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
                return NotFound("Staff not found.");

            // Auto-close any forgotten open session from a previous day
            var oldOpenSession = await _context.StaffAttendances
                .Where(a => a.StaffId == request.StaffId && a.ClockOutTime == null)
                .OrderByDescending(a => a.ClockInTime)
                .FirstOrDefaultAsync();

            if (oldOpenSession != null && oldOpenSession.ClockInTime.HasValue)
            {
                DateTime oldClockInLocal = oldOpenSession.ClockInTime.Value.AddHours(2);
                DateTime nowLocalCheck = DateTime.UtcNow.AddHours(2);

                // Only auto-close if it's from a previous calendar day (not today's still-open session)
                if (oldClockInLocal.Date < nowLocalCheck.Date)
                {
                    DateTime shiftEndLocal = oldClockInLocal.Date.AddHours(16); // 4:00 PM — actual paid shift end
                    DateTime shiftEndUtc = shiftEndLocal.AddHours(-2);

                    oldOpenSession.ClockOutTime = shiftEndUtc;
                    oldOpenSession.IsAnomaly = true;
                    oldOpenSession.AnomalyReason = "Auto clocked out — staff forgot to clock out on a previous day";
                    var oldHours = (shiftEndUtc - oldOpenSession.ClockInTime.Value).TotalHours;
                    oldOpenSession.TotalHoursWorked = (decimal)oldHours;

                    await _context.SaveChangesAsync();
                }
            }

            byte[]? photoBytes = null;
            if (!string.IsNullOrEmpty(request.PhotoBase64))
                photoBytes = Convert.FromBase64String(request.PhotoBase64);

            DateTime clockInTimeUtc = DateTime.UtcNow; //dont forget to convert times first before intro anomaly
            DateTime clockInTimeLocal = clockInTimeUtc.AddHours(2);
            TimeSpan expectedStartTime = new TimeSpan(8, 0, 0);
            TimeSpan graceWindow = TimeSpan.FromMinutes(10);
            bool isLate = clockInTimeLocal.TimeOfDay > (expectedStartTime + graceWindow);

            var attendance = new StaffAttendance
            {
                StaffId = request.StaffId,
                ClockInTime = clockInTimeUtc,
                ClockInLatitude = request.Latitude,
                ClockInLongitude = request.Longitude,
                ClockInPhoto = photoBytes,
                IsAnomaly = isLate,
                AnomalyReason = isLate ? $"Late clock-in — arrived at {clockInTimeLocal:HH:mm}, expected by {expectedStartTime.Add(graceWindow):hh\\:mm}" : null
            };

            _context.StaffAttendances.Add(attendance);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Clock-in successful.", attendanceId = attendance.Id, isAnomaly = isLate });
        }

        [HttpPost("ClockOut")]
        public async Task<IActionResult> ClockOut([FromBody] ClockInRequest request)
        {
            var staff = await _context.Staffs.FindAsync(request.StaffId);
            if (staff == null)
                return NotFound("Staff not found.");

            var attendance = await _context.StaffAttendances
                .Where(a => a.StaffId == request.StaffId && a.ClockOutTime == null)
                .OrderByDescending(a => a.ClockInTime)
                .FirstOrDefaultAsync();

            if (attendance == null)
                return BadRequest("No clock-in record found for this staff.");

            byte[]? photoBytes = null;
            if (!string.IsNullOrEmpty(request.PhotoBase64))
                photoBytes = Convert.FromBase64String(request.PhotoBase64);

            attendance.ClockOutTime = DateTime.UtcNow;
            attendance.ClockOutLatitude = request.Latitude;
            attendance.ClockOutLongitude = request.Longitude;
            attendance.ClockOutPhoto = photoBytes;

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