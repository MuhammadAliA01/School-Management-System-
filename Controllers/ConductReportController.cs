using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.ServicesAi;

namespace SchoolManagmentSystem.Controllers
{
    public class ConductReportController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly GeminiService _geminiService = new GeminiService();

        public ConductReportController(ApplicationDBContext context)
        {
            _context = context;
        }

        // 1. DASHBOARD
        public async Task<IActionResult> Index()
        {
            var reports = await _context.ConductReports
                .Include(c => c.Student)
                .OrderByDescending(c => c.IncidentDate)
                .ToListAsync();
            return View(reports);
        }

        // 2. VIEW DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.ConductReports
                .Include(c => c.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (report == null) return NotFound();

            return View(report);
        }

        // 3. GET: CREATE FORM
        public IActionResult Create()
        {
            ViewBag.StudentList = new SelectList(_context.Students, "StudentId", "FullName");
            return View("LogConduct");
        }

        // 4. POST: SAVE & AI GENERATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConductReport report)
        {
            ModelState.Remove("Student");
            ModelState.Remove("InterventionPlan");

            if (ModelState.IsValid)
            {
                report.InterventionPlan = await _geminiService.GetConductPlan(report.Description, report.Severity);
                _context.Add(report);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.StudentList = new SelectList(_context.Students, "StudentId", "FullName", report.StudentId);
            return View("LogConduct", report);
        }

        // 5. DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.ConductReports.FindAsync(id);
            if (report != null)
            {
                _context.ConductReports.Remove(report);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // 6. PARENT VIEW (Unified GET Method)
        // This handles the "Full Doc" view and the "Edit" mode toggle
        [HttpGet]
        public async Task<IActionResult> ParentView(int id, bool edit = false)
        {
            var report = await _context.ConductReports
                .Include(c => c.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (report == null) return NotFound();

            // Logic: If they haven't viewed it yet, mark it now
            if (!report.IsViewedByParent)
            {
                report.IsViewedByParent = true;
                report.DateViewed = DateTime.Now;
                _context.Update(report);
                await _context.SaveChangesAsync();
            }

            ViewBag.IsEditing = edit;
            return View(report);
        }

        // 7. SUBMIT PARENT RESPONSE (Unique POST Method)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitParentResponse(int id, string parentMessage)
        {
            var report = await _context.ConductReports.FindAsync(id);
            if (report == null) return NotFound();

            report.ParentMessage = parentMessage;
            report.IsViewedByParent = true;

            _context.Update(report);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ParentView), new { id = report.Id });
        }

        // 8. GET: EDIT (Teacher/Admin Side)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.ConductReports.FindAsync(id);
            if (report == null) return NotFound();

            ViewBag.StudentList = new SelectList(_context.Students, "StudentId", "FullName", report.StudentId);
            return View(report);
        }

        // 9. POST: EDIT (Teacher/Admin Side)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,InterventionPlan,Severity,IncidentDate,StudentId,IsViewedByParent,ParentMessage")] ConductReport report)
        {
            if (id != report.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(report);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportExists(report.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(report);
        }

        // 10. HELPER METHOD
        private bool ReportExists(int id)
        {
            return _context.ConductReports.Any(e => e.Id == id);
        }
    }
}