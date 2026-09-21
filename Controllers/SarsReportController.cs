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
            _reportService = new SarsReportService(context);
        }

        [HttpGet]
        public IActionResult GenerateReport(DateTime payCycle)
        {
            var report = _reportService.GenerateReport(payCycle);
            return View(report);
        }
    }
}