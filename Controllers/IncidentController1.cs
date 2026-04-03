using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
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

        // GET: Incident/Create
        public IActionResult Create()
        {
            var teachers = _context.Teachers.ToList();
            ViewBag.Teachers = new SelectList(teachers, "TeacherId", "Name");
            return View("ReportIncident");
        }

        // --- STEP 2: THE ROSTER FETCHER ---
        [HttpGet]
        public JsonResult GetStudentsByTeacher(int teacherId)
        {
            // This grabs only the students belonging to the teacher you clicked
            var roster = _context.Students
                .Where(s => s.TeacherId == teacherId)
                .Select(s => new {
                    id = s.StudentId,
                    name = s.FullName
                })
                .ToList();

            return Json(roster);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Incident model)
        {/*
            if (ModelState.IsValid)
            {
                _context.Incidents.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            */
            ViewBag.Teachers = new SelectList(_context.Teachers.ToList(), "TeacherId", "Name");
            return View("ReportIncident", model);
        }

        public IActionResult Index()
        {
           /* var incidents = _context.Incidents.ToList();*/
            return View();
        }
    }
}
