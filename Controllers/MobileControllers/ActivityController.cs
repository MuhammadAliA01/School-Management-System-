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
                .Join(
                    _context.Students,
                    activity => activity.StudentID,
                    student => student.StudentId,
                    (activity, student) => new
                    {
                        activity,
                        student
                    }
                )
                .Where(x => x.student.ParentId == parentID)
                .Select(x => new
                {
                    x.activity.ActivityID,
                    x.activity.StudentID,
                    StudentName = x.student.FullName,
                    x.activity.TeacherID,
                    x.activity.HomeworkDescription,
                    x.activity.Grade,
                    x.activity.GradedAt
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
                        activity.HomeworkDescription,
                        activity.Grade,
                        activity.GradedAt
                    }
                )
                .ToListAsync();

            return Ok(activities);
        }

        [HttpPut("grade/{activityID}")]
        public async Task<IActionResult> GradeActivity(
    int activityID,
    [FromBody] GradeActivityRequest request)
        {
            if (request.Grade < 1 || request.Grade > 5)
            {
                return BadRequest("Grade must be between 1 and 5.");
            }

            var activity = await _context.ActivityModels
                .FirstOrDefaultAsync(a => a.ActivityID == activityID);

            if (activity == null)
            {
                return NotFound("Activity not found.");
            }

            activity.Grade = request.Grade;
            activity.GradedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Activity graded successfully.",
                activityID = activity.ActivityID,
                grade = activity.Grade,
                gradedAt = activity.GradedAt
            });
        }
    }
}
