using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;

namespace SchoolManagmentSystem.Controllers
{
    public class SarsReportController : Controller
    {
        private readonly SarsReportService _reportService;
        private readonly SarsReportDocumentService _pdfService;

        public SarsReportController(ApplicationDBContext context)
        {
            _reportService = new SarsReportService(context);
            _pdfService = new SarsReportDocumentService();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GenerateReport(DateTime payCycle)
        {
            var report = _reportService.GenerateReport(payCycle);
            var pdfBytes = _pdfService.GeneratePdf(report);

            string fileName = $"SARS_Report_{payCycle:yyyy_MM}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}