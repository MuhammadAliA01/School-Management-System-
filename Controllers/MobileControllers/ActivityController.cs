using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;

namespace SchoolManagmentSystem.Controllers.MobileControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {

        private readonly ApplicationDBContext _context;
        private readonly FirebaseNotificationService _firebaseNotificationService;

        public ActivityController(
            ApplicationDBContext context,
            FirebaseNotificationService firebaseNotificationService)
        {
            _context = context;
            _firebaseNotificationService = firebaseNotificationService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateActivity(
    [FromBody] CreateActivityRequest request)
        {
            foreach (var studentID in request.StudentIDs)
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.StudentId == studentID);

                if (student == null)
                {
                    continue;
                }

                var activity = new ActivityModel
                {
                    StudentID = studentID,
                    TeacherID = request.TeacherID,
                    HomeworkDescription = request.HomeworkDescription
                };

                _context.ActivityModels.Add(activity);

                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Id == student.ParentId);

                if (parent != null && !string.IsNullOrEmpty(parent.FCMToken))
                {
                    await _firebaseNotificationService.SendNotification(
                        parent.FCMToken,
                        "New Homework",
                        $"{student.FullName} has been assigned new homework."
                    );
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Activity assigned successfully");
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _context.Students
                .Select(s => new
                {
                    studentID = s.StudentId,
                    firstName = s.FullName,
                })
                .ToListAsync();

            return Ok(students);
        }

        [HttpGet("parent/{parentID}")]
        public async Task<IActionResult> GetParentActivities(int parentID)
        {
            var activities = await _context.ActivityModels
                .Where(a => _context.Students
                    .Any(s =>
                        s.StudentId == a.StudentID &&
                        s.ParentId == parentID
                    ))
                .Select(a => new
                {
                    a.ActivityID,
                    a.StudentID,
                    a.TeacherID,
                    a.HomeworkDescription
                })
                .ToListAsync();

            return Ok(activities);
        }

        [HttpGet("teacher/{teacherID}")]
        public async Task<IActionResult> GetTeacherActivities(int teacherID)
        {
            var activities = await _context.ActivityModels
                .Where(a => a.TeacherID == teacherID)
                .Join(
                    _context.Students,
                    activity => activity.StudentID,
                    student => student.StudentId,
                    (activity, student) => new
                    {
                        activity.ActivityID,
                        activity.StudentID,
                        StudentName = student.FullName,
                        activity.TeacherID,
                        activity.HomeworkDescription
                    }
                )
                .ToListAsync();

            return Ok(activities);
        }
    }
}
