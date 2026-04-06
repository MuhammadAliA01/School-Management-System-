using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagmentSystem.Controllers
{
    public class IncidentController : Controller
    {
        private readonly ApplicationDBContext _context;

        public IncidentController(ApplicationDBContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index(string? searchString)
        {
            var query = _context.Incidents
                .Include(i => i.Teacher)
                .Include(i => i.Student)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                // We use the ! operator to tell the compiler the Student and FullName are not null
                query = query.Where(s => s.Student != null && s.Student!.FullName!.Contains(searchString));
            }

            var groupedIncidents = query
                .OrderByDescending(i => i.Date)
                .ToList()
                .GroupBy(i => i.Teacher?.Name ?? "General Staff")
                .ToList();

            ViewData["CurrentFilter"] = searchString;
            return View("ReportView", groupedIncidents);
        }

        [Authorize]
        public IActionResult Create()
        {
            var model = new Incident { Date = DateTime.Today };
            var teachers = _context.Teachers.OrderBy(t => t.Name).ToList();
            ViewBag.Teachers = new SelectList(teachers, "TeacherId", "Name");
            return View("ReportIncident", model);
        }

        [HttpGet]
        public JsonResult GetStudentsByTeacher(int teacherId)
        {
            var roster = _context.Students
                .Where(s => s.TeacherId == teacherId)
                .OrderBy(s => s.FullName)
                .Select(s => new { id = s.StudentId, name = s.FullName })
                .ToList();
            return Json(roster);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Incident model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Incidents.Add(model);
                    _context.SaveChanges();
                    TempData["AlertMessage"] = "Incident logged successfully.";
                    TempData["AlertType"] = "success";
                    return RedirectToAction("Index");
                }
                catch
                {
                    TempData["AlertMessage"] = "An error occurred while saving.";
                    TempData["AlertType"] = "danger";
                }
            }

           var teachers = _context.Teachers.OrderBy(t => t.Name).ToList();
           ViewBag.Teachers = new SelectList(teachers, "TeacherId", "Name");
            return View("ReportIncident", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var incident = _context.Incidents.Find(id);
            if (incident != null)
            {
                _context.Incidents.Remove(incident);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}