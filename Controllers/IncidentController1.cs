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
            var query = _context.Incidents.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(i => i.StudentName != null && i.StudentName.Contains(searchString));
            }

            var groupedIncidents = query
                .OrderByDescending(i => i.Date)
                .ToList()
                .GroupBy(i => i.TeacherName ?? "General Staff")
                .ToList();

            ViewData["CurrentFilter"] = searchString;
            return View("ReportView", groupedIncidents);
        }

        [Authorize]
        public IActionResult Create()
        {
            var model = new Incident { Date = DateTime.Today };
            var teachers = _context.Staffs.OrderBy(t => t.FirstName).Where(t => t.Role == "Teacher").ToList();
            ViewBag.Teachers = new SelectList(teachers, "id", "FirstName");
            return View("ReportIncident", model);
        }

        [HttpGet]
        public JsonResult GetAllStudents()
        {
            var students = _context.Students
        .Select(s => new {
            id = s.StudentId,
            name = s.FullName
        })
        .ToList();

            return Json(students);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Incident model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.StudentName = _context.Students
   .Where(s => s.StudentId == model.StudentId)
   .Select(s => s.FullName)
   .FirstOrDefault();

                    model.TeacherName = _context.Staffs
                        .Where(t => t.id == model.TeacherId)
                        .Select(t => t.FirstName)
                        .FirstOrDefault();
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

            var teachers = _context.Staffs.OrderBy(t => t.FirstName).Where(t => t.Role == "Teacher").ToList();
            ViewBag.Teachers = new SelectList(teachers, "id", "FirstName");
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