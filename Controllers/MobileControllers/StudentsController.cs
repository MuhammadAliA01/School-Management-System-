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

    [HttpGet("GetClassStudents")]
    public async Task<IActionResult> GetClassStudents(int teacherId, int classId)
    {
        // 1. Query database filtering by both TeacherId and ClassId
        var students = await _context.Students
            .Where(s => s.TeacherId == teacherId && s.ClassId == classId)
            .Select(s => new
            {
                s.StudentId,
                s.FullName
            })
            .ToListAsync();

        // 2. Return JSON list
        return Ok(students);
    }
}