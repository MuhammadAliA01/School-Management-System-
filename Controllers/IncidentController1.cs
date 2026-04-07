using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.ServicesAi.IncidentAI;
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

        public async Task<IActionResult> Details(int id)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident == null) return NotFound();

            var service = new IncidentAnalysisService();
            var analysis = await service.AnalyzeSingleIncidentAsync(incident);

            ViewBag.AIAnalysis = analysis;
            return View(incident);
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
        public async Task<IActionResult> Create(Incident model) // Change to async Task
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Fetch names as you were doing
                    model.StudentName = _context.Students
                        .Where(s => s.StudentId == model.StudentId)
                        .Select(s => s.FullName)
                        .FirstOrDefault();

                    model.TeacherName = _context.Staffs
                        .Where(t => t.id == model.TeacherId)
                        .Select(t => t.FirstName)
                        .FirstOrDefault();

                    // 2. TRIGGER THE AI ANALYSIS
                    // We do this BEFORE saving so the analysis is stored with the record
                    var aiService = new IncidentAnalysisService();
                    var analysis = await aiService.AnalyzeSingleIncidentAsync(model);

                    // 3. Map the AI results to your model
                    // Make sure these columns exist in your Incidents table!
                    model.AISeverity = analysis.SeverityLevel;
                    model.AIImpact = analysis.Impact;
                    model.AIEvaluation = analysis.ActionEvaluation;
                    model.AIAdvice = analysis.ProfessionalAdvice;

                    // 4. Save everything
                    _context.Incidents.Add(model);
                    await _context.SaveChangesAsync(); // Use Async version

                    TempData["AlertMessage"] = "Incident logged and AI analyzed successfully.";
                    TempData["AlertType"] = "success";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Log ex.Message here to debug if AI fails
                    TempData["AlertMessage"] = "An error occurred while saving.";
                    TempData["AlertType"] = "danger";
                }
            }

            // If we got here, something failed; reload the teacher list for the view
            var teachers = _context.Staffs
                .OrderBy(t => t.FirstName)
                .Where(t => t.Role == "Teacher")
                .ToList();
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