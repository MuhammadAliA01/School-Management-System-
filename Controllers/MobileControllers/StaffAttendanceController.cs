using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;

namespace SchoolManagmentSystem.Controllers.MobileControllers
{
    public class ClockInRequest
    {
        public int StaffId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public IFormFile? Photo { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]

    public class StaffAttendanceController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly FirebaseNotificationService _firebaseNotificationService;

        public StaffAttendanceController(ApplicationDBContext context, IWebHostEnvironment env, FirebaseNotificationService firebaseNotificationService)
        {
            _context = context;
            _env = env;
            _firebaseNotificationService = firebaseNotificationService;

        }

        private async Task<string?> SaveFileAsync(IFormFile? file, int staffId, string type)
        {
            if (file == null || file.Length == 0) return null;

            // Define path: wwwroot/uploads/images/
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Create unique filename: staff1ClockIn_20260804212800.jpg
            var fileName = $"staff{staffId}{type}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path for the database
            return $"/uploads/images/{fileName}";
        }

        [HttpPost("ClockIn")]
        public async Task<IActionResult> ClockIn([FromForm] ClockInRequest request) 
        {
            var staff = await _context.Staffs.FindAsync(request.StaffId);
            if (staff == null)
                return NotFound("Staff not found.");

            DateTime todayLocal = DateTime.UtcNow.AddHours(2).Date; // SAT

            var alreadyClockedInToday = await _context.StaffAttendances
                .Where(a => a.StaffId == request.StaffId
                         && a.ClockInTime.HasValue
                         && a.ClockInTime.Value.AddHours(2).Date == todayLocal)
                .AnyAsync();

            if (alreadyClockedInToday)
            {
                return BadRequest(new { message = "You have already clocked in today. Only one clock-in per day is allowed." });
            }

            // Auto-close any forgotten open session from a previous day
            var oldOpenSession = await _context.StaffAttendances
                .Where(a => a.StaffId == request.StaffId && a.ClockOutTime == null)
                .OrderByDescending(a => a.ClockInTime)
                .FirstOrDefaultAsync();

            if (oldOpenSession != null && oldOpenSession.ClockInTime.HasValue)
            {
                DateTime oldClockInLocal = oldOpenSession.ClockInTime.Value.AddHours(2);
                DateTime nowLocalCheck = DateTime.UtcNow.AddHours(2);

                
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

            string? photoUrl = await SaveFileAsync(request.Photo, request.StaffId, "ClockIn");

            DateTime clockInTimeUtc = DateTime.UtcNow;
            DateTime clockInTimeLocal = clockInTimeUtc.AddHours(2); // Account for SAT
            TimeSpan expectedStartTime = new TimeSpan(8, 0, 0);
            TimeSpan graceWindow = TimeSpan.FromMinutes(10);
            bool isLate = clockInTimeLocal.TimeOfDay > (expectedStartTime + graceWindow);

            var attendance = new StaffAttendance
            {
                StaffId = request.StaffId,
                ClockInTime = clockInTimeUtc,
                ClockInLatitude = (decimal)request.Latitude,   
                ClockInLongitude = (decimal)request.Longitude, 
                ClockInPhotoUrl = photoUrl, // Saving the file path here
                IsAnomaly = isLate,
                AnomalyReason = isLate ? $"Late clock-in — arrived at {clockInTimeLocal:HH:mm}, expected by {expectedStartTime.Add(graceWindow):hh\\:mm}" : null
            };

            _context.StaffAttendances.Add(attendance);
            await _context.SaveChangesAsync();

            

            if (staff != null && !string.IsNullOrEmpty(staff.FCMToken))
            {
                await _firebaseNotificationService.SendNotification(
                    staff.FCMToken,
                    "Clocked In",
                    "You have Clocked In"
                );
            }
            return Ok(new { message = "Clock-in successful.", attendanceId = attendance.Id, isAnomaly = isLate });
        }

        [HttpPost("ClockOut")]
        public async Task<IActionResult> ClockOut([FromForm] ClockInRequest request) // Use FromForm!
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

            
            string? photoUrl = await SaveFileAsync(request.Photo, request.StaffId, "ClockOut");

            attendance.ClockOutTime = DateTime.UtcNow;
            attendance.ClockOutLatitude = (decimal)request.Latitude; 
            attendance.ClockOutLongitude = (decimal)request.Longitude; 
            attendance.ClockOutPhotoUrl = photoUrl; // Saving the file path here

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