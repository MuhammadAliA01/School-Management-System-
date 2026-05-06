using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers
{
    [Authorize]
    public class StudentProgressVisualizationController : Controller
    {
        private readonly ApplicationDBContext _context;

        public StudentProgressVisualizationController(ApplicationDBContext context)
        {
            _context = context;
        }

        // Returns the page
        public IActionResult Index(int studentId)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentId == studentId);
            if (student == null) return NotFound();

            ViewBag.StudentName = student.FullName;
            ViewBag.StudentId = studentId;
            return View();
        }

        [HttpGet]
        public IActionResult GetChartData(int studentId)
        {
            var subjects = new[]
            {
                "Creative Development",
                "Oral skills and Language",
                "Personal Independence",
                "Basic Concepts",
                "Physical and Social",
                "Social Skills"
            };

            var assessments = _context.StudentAssessments
                .Where(a => a.StudentId == studentId)
                .ToList();

            var allAssessments = _context.StudentAssessments.ToList();

            // Chart 1 — weekly scores per subject per term
            var weeklyData = assessments
                .GroupBy(a => new { a.SubjectName, a.TermNumber, a.WeekNumber })
                .Select(g => new {
                    subject = g.Key.SubjectName,
                    term = g.Key.TermNumber,
                    week = g.Key.WeekNumber,
                    avg = Math.Round(g.Average(a => a.Score), 1)
                })
                .OrderBy(x => x.term).ThenBy(x => x.week)
                .ToList();

            // Chart 2 — average per subject
            var subjectAverages = subjects.Select(s => new {
                subject = s,
                avg = assessments.Where(a => a.SubjectName == s).Any()
                    ? Math.Round(assessments.Where(a => a.SubjectName == s).Average(a => a.Score), 1)
                    : 0.0
            }).ToList();

            // Chart 3 — student vs class average per subject
            var classAverages = subjects.Select(s => new {
                subject = s,
                avg = allAssessments.Where(a => a.SubjectName == s).Any()
                    ? Math.Round(allAssessments.Where(a => a.SubjectName == s).Average(a => a.Score), 1)
                    : 0.0
            }).ToList();

            // Chart 4 — term over term per subject
            var termData = assessments
                .GroupBy(a => new { a.SubjectName, a.TermNumber })
                .Select(g => new {
                    subject = g.Key.SubjectName,
                    term = g.Key.TermNumber,
                    avg = Math.Round(g.Average(a => a.Score), 1)
                })
                .ToList();

            
            var overallAvg = assessments.Any() ? Math.Round(assessments.Average(a => a.Score), 1) : 0.0;
            var bestSubject = subjectAverages.OrderByDescending(s => s.avg).FirstOrDefault()?.subject ?? "N/A";
            var mostImproved = assessments
                .GroupBy(a => a.SubjectName)
                .Where(g => g.Count() >= 2)
                .Select(g => new {
                    subject = g.Key,
                    improvement = g.OrderByDescending(a => a.TermNumber).ThenByDescending(a => a.WeekNumber).First().Score
                                - g.OrderBy(a => a.TermNumber).ThenBy(a => a.WeekNumber).First().Score
                })
                .OrderByDescending(x => x.improvement)
                .FirstOrDefault()?.subject ?? "N/A";

            return Json(new
            {
                weeklyData,
                subjectAverages,
                classAverages,
                termData,
                overallAvg,
                bestSubject,
                mostImproved,
                subjects
            });
        }
    }
}