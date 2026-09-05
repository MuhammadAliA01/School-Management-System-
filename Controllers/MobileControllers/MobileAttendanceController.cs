using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;

namespace SchoolManagmentSystem.Controllers.MobileControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileAttendanceController : ControllerBase
    {

        private readonly ApplicationDBContext _context;
        private readonly FirebaseNotificationService _firebaseNotificationService;


        public MobileAttendanceController(ApplicationDBContext context, FirebaseNotificationService firebaseNotificationService)
        {
            _context = context;
            _firebaseNotificationService = firebaseNotificationService;


        }


        [HttpGet("classlist")]
        public async Task<IActionResult> ClassList()
        {
            var listStudents = _context.Students.ToList();

            List<StudentAttTemp> students = new List<StudentAttTemp>();

            foreach(Student st in listStudents)
            {
                var studentInAttStatus = _context.AttendanceModels.Where(x => x.studentID == st.StudentId && x.DateAttended == DateTime.Today).Select(x => x.status).FirstOrDefault();

                if (studentInAttStatus == null)
                {
                    StudentAttTemp student = new StudentAttTemp
                    {
                        studentID = st.StudentId,
                        name = st.FullName,
                        status = "Absent"
                    };
                    students.Add(student);

                }
                else
                {
                    StudentAttTemp student = new StudentAttTemp
                    {
                        studentID = st.StudentId,
                        name = st.FullName,
                        status = studentInAttStatus
                    };

                    students.Add(student);

                }


            }

            return Ok(students);


        }

        [HttpPost("logattendance")]
        public async Task<IActionResult> TakeAttendance([FromBody]AttendanceRequest request)
        {
            var student = _context.Students.Where(x => x.StudentId == request.studentID).Select(x => x).FirstOrDefault();



            var teacher = _context.Staffs.Where(x => x.id == request.teacherID).Select(x => x).FirstOrDefault();

            var studentExists = _context.AttendanceModels.Where(x => x.studentID == request.studentID && x.DateAttended == DateTime.Today).Select(x => x).FirstOrDefault();




            if (student == null)
            {
                return NotFound();
            }

            if (studentExists == null)
            {
                var attendance = new AttendanceModel
                {
                    studentID = student.StudentId,
                    teacherID = teacher.id,
                    DateAttended = DateTime.Now.Date,
                    status = "Present"

                };

                _context.AttendanceModels.Add(attendance);
                await _context.SaveChangesAsync();

                var parent = await _context.Parents
                    .FirstOrDefaultAsync(p => p.Id == student.ParentId);

                if (parent != null && !string.IsNullOrEmpty(parent.FCMToken))
                {
                    bool success = await _firebaseNotificationService.SendNotification(
                     parent.FCMToken,
                    "Attendance Taken",
                 $"{student.FullName} attendance has been taken."
                                        );

                    if (!success)
                    {
                        parent.FCMToken = null;
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok();
            }
            else if (studentExists != null)
            {
                return Ok("Attendance Already taken");

                
            }
            




              return NotFound("error");

            

        }


        [HttpPost("checkattendance")]
        public async Task<IActionResult> CheckAttendance([FromBody] CheckAttRequest request)
        {


            var studentExists = _context.AttendanceModels.Where(x => x.studentID == request.studentID && x.DateAttended == DateTime.Today).Select(x => x).FirstOrDefault();


          

            if (studentExists == null)
            {
                

                return Ok("Correct");
            }
            else if (studentExists != null)
            {
                return Ok("Attendance Already taken");


            }





            return NotFound("error");



        }



    }
}
