using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers.MobileControllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class MobileStaffScheduleController : ControllerBase
    {

        private readonly ApplicationDBContext _context;

        public MobileStaffScheduleController(ApplicationDBContext context)
        {
            _context = context;
        }


        [HttpGet("staff/{staffId}")]
        public async Task<IActionResult> GetStaffSchedule(int staffId)
        {
            var schedules = await _context.StaffSchedules
                .Where(s => s.TeacherId == staffId)
                .OrderBy(s => s.ScheduleDate)
                .Select(s => new
                {
                    scheduleId = s.ScheduleId,
                    teacherId = s.TeacherId,
                    classId = s.ClassId,
                    className = _context.Classes
                        .Where(c => c.ClassId == s.ClassId)
                        .Select(c => c.ClassName)
                        .FirstOrDefault(),
                    scheduleDate = s.ScheduleDate,
                    shiftType = s.ShiftType
                })
                .ToListAsync();

            return Ok(schedules);
        }
    }
}

