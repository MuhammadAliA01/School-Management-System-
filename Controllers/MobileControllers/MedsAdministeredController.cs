using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedsAdministeredController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public MedsAdministeredController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("MedsAdministeredController is working!");
        }


        // GET /api/MedsAdministered/students
        // Gets the list of students for Flutter
        [HttpGet("students")]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _context.Students
                .Select(s => new
                {
                    id = s.StudentId,
                    name = s.FullName}).ToListAsync();

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
    }
}
        