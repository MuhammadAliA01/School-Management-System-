using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;

namespace SchoolManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedsAdministeredController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly FirebaseNotificationService _firebaseNotificationService;


        public MedsAdministeredController(ApplicationDBContext context, FirebaseNotificationService firebaseNotificationService)
        {
            _context = context;
            _firebaseNotificationService = firebaseNotificationService;

        }



        // GET /api/MedsAdministered/students
        // Gets the list of students for Flutter
        [HttpGet("students")]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _context.Students
                .Include(s => s.Parent)
                .Include(s => s.AssignedClassStudents)
                .Select(s => new
                {
                    id = s.StudentId,
                    name = s.FullName,
                    parentName = s.Parent != null
                        ? s.Parent.FirstName + " " + s.Parent.Surname
                        : "No parent",
                    className = s.AssignedClassStudents != null
                        ? s.AssignedClassStudents.ClassName
                        : "No class"
                })
                .ToListAsync();

            return Ok(students);
        }


        // GET /api/MedsAdministered/students/{studentId}/parent
        // Gets the parent info for a specific student
        [HttpGet("students/{studentId}/parent")]
        public async Task<IActionResult> GetStudentParent(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.Parent)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            if (student.Parent == null)
            {
                return NotFound("Parent not found for this student.");
            }

            return Ok(new
            {
                parentId = student.Parent.Id,
                firstName = student.Parent.FirstName,
                surname = student.Parent.Surname,
                email = student.Parent.Email
            });
        }


        // POST /api/MedsAdministered
        // Saves the meds administration record & also returns students parents info
        [HttpPost]
        public async Task<IActionResult> AdministerMedication(
         [FromBody] MedsAdministeredRequest request)
        {
            // Check if the student exists and get their parent
            var student = await _context.Students
                .Include(s => s.Parent)
                .FirstOrDefaultAsync(s => s.StudentId == request.studentID);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            // Check if the teacher/staff member exists
            var teacherExists = await _context.Staffs
                .AnyAsync(s => s.id == request.teacherID);

            if (!teacherExists)
            {
                return NotFound("Teacher not found.");
            }

            // Check if the medication exists
            var medicationExists = await _context.Medications
                .AnyAsync(m => m.ID == request.medID);

            var medicationName = _context.Medications
                .Where(m => m.ID == request.medID).Select(m => m.medName).FirstOrDefault();

            if (!medicationExists)
            {
                return NotFound("Medication not found.");
            }

            // Create the medication administration record
            var medicationAdministered = new medsAdministered
            {
                studentID = request.studentID,
                teacherID = request.teacherID,
                medID = request.medID,
                dateAdministered = request.dateAdministered
            };

            // Save the medication record
            _context.medsAdministered.Add(medicationAdministered);

            await _context.SaveChangesAsync();

            // Check if the student has a parent
            if (student.Parent == null)
            {
                return Ok(new
                {
                    message = "Medication administered successfully, but no parent was found."
                });
            }

            if (student.Parent != null && !string.IsNullOrEmpty(student.Parent.FCMToken))
            {
                await _firebaseNotificationService.SendNotification(
                    student.Parent.FCMToken,
                    "Medication Administered",
                    $"{student.FullName} has been given {medicationName}"
                );
            }


            // Return the medication confirmation and parent info
            return Ok(new
            {
                message = "Medication administered successfully.",

                parent = new
                {
                    parentId = student.Parent.Id,
                    firstName = student.Parent.FirstName,
                    surname = student.Parent.Surname,
                    email = student.Parent.Email
                }
            });
        }

        // GET /api/MedsAdministered/student/{studentId}/history
        // Gets all medication records for a specific student
        [HttpGet("student/{studentId}/history")]
        public async Task<IActionResult> GetMedicationHistory(int studentId)
        {
            var history = await _context.medsAdministered
                .Where(m => m.studentID == studentId)
                .OrderByDescending(m => m.dateAdministered)
                .Select(m => new
                {
                    studentId = m.studentID,
                    medicationName = m.medication.medName,
                    dateAdministered = m.dateAdministered
                })
                .ToListAsync();

            return Ok(history);

        }
        [HttpGet("parent/{parentId}/student")]
        public async Task<IActionResult> GetStudentForParent(int parentId)
        {
            var student = await _context.Students
                .Where(s => s.ParentId == parentId)
                .Select(s => new
                {
                    studentId = s.StudentId,
                    studentName = s.FullName
                })
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound("No student found for this parent.");
            }

            return Ok(student);
        }
    }
}
        