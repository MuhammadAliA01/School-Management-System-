using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly ApplicationDBContext _context;

    public StudentsController(ApplicationDBContext context)
    {
        _context = context;
    }

    [HttpGet("GetTeacherStudents")]
    public async Task<IActionResult> GetTeacherStudents(int teacherId)
    {
        var students = await _context.Students
            .Where(s => s.TeacherId == teacherId)
            .Select(s => new {
                studentId = s.StudentId,
                fullName = s.FullName
            })
            .ToListAsync();

        return Ok(students);
    }
}