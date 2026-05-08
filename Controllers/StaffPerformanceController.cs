using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers
{
    [Authorize]
    public class StaffPerformanceController : Controller
    {
        private readonly ApplicationDBContext _context;

        public StaffPerformanceController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var allStaff = _context.Staffs.ToList();
            var allAssessments = _context.StudentAssessments.ToList();

            var performanceData = allStaff.Select(s => new StaffPerformanceViewModel
            {
                StaffId = s.id,
                FullName = s.FirstName + " " + s.Surname,
                Role = s.Role,

                // Average score of all assessments logged by this therapist
                AverageScore = allAssessments.Where(a => a.TeacherId == s.id).Any()
                    ? Math.Round(allAssessments.Where(a => a.TeacherId == s.id).Average(a => a.Score), 1)
                    : 0,

                // How many distinct term/week combinations they logged in
                WeeksLogged = allAssessments.Where(a => a.TeacherId == s.id)
                    .Select(a => new { a.TermNumber, a.WeekNumber })
                    .Distinct()
                    .Count(),

                // Total assessments logged
                TotalAssessments = allAssessments.Count(a => a.TeacherId == s.id)

            }).ToList();

            return View(performanceData);
        }
    }
}