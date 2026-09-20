using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;

namespace SchoolManagmentSystem.Controllers
{
    public class SarsReportController : Controller
    {
        private readonly SarsReportService _reportService;

        public SarsReportController(ApplicationDBContext context)
        {
            _reportService = new SarsReportService(new MockSarsTaxDataProvider(), context);
        }

        [HttpGet]
        public IActionResult GenerateReport(DateTime payCycle)
        {
            var staffIds = _reportService.GetStaffIdsForPeriod(payCycle);

            if (!staffIds.Any())
            {
                return NotFound("No earnings found for this pay period.");
            }

            SarsMonthlyReportDTO report = _reportService.GenerateReport(staffIds, payCycle);

            return View(report);
        }
    }
}