using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers.MobileControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileAttendanceController : ControllerBase
    {

        private readonly ApplicationDBContext _context;

        public MobileAttendanceController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost("logattendance")]
        public async Task<IActionResult> TakeAttendance([FromBody]AttendanceRequest request)
        {
            var student = _context.Students.Where(x => x.StudentId == request.studentID).Select(x => x).FirstOrDefault();

            var teacher = _context.Staffs.Where(x => x.id == request.teacherID).Select(x => x).FirstOrDefault();


            if (student == null)
            {
                return NotFound();
            }

            var attendance = new AttendanceModel
            {
                studentID = student.StudentId,
                teacherID = teacher.id,
                DateAttended = DateTime.Now.Date,
                status = "Present"

            };

            _context.AttendanceModels.Add(attendance);
            _context.SaveChangesAsync();

            return Ok();

            

        }
    }
}
