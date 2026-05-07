using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class IEPController : Controller
{
    private readonly ApplicationDBContext _context;

    public IEPController(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var ieps = await _context.IEPs
            .Include(i => i.Student)
            .OrderByDescending(i => i.DateCreated)
            .ToListAsync();
        return View(ieps);
    }

    public IActionResult Create()
    {
        // Get the list of students
        var students = _context.Students.ToList();

        // Use "StudentId" as the value field
        ViewBag.StudentList = new SelectList(students, "StudentId", "FullName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IEP iep, int PeriodAmount, string PeriodUnit)
    {
        // 1. Combine the duration into the Term string
        iep.Term = $"{PeriodAmount} {PeriodUnit}";

        // 2. Ensure the date created is set
        iep.DateCreated = DateTime.Now;

        // 3. Remove validation for 'Student' and 'Term' if they are causing ModelState.IsValid to be false
        // Razor sometimes complains about navigation properties like 'Student' being null.
        ModelState.Remove("Student");
        ModelState.Remove("Term");

        if (ModelState.IsValid)
        {
            _context.Add(iep);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 4. IF WE ARE HERE: The form failed. We MUST reload the list using "StudentId"
        var studentsList = _context.Students.ToList();
        ViewBag.StudentList = new SelectList(studentsList, "StudentId", "FullName", iep.StudentId);

        return View(iep);
    }

    // POST: IEP/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var iep = await _context.IEPs.FindAsync(id);
        if (iep != null)
        {
            _context.IEPs.Remove(iep);
            await _context.SaveChangesAsync();
            TempData["Success"] = "IEP deleted successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var iep = await _context.IEPs
            .Include(i => i.Student)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (iep == null) return NotFound();

        return View(iep);
    }
}