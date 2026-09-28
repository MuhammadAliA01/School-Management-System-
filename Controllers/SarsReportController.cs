using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;

namespace SchoolManagmentSystem.Controllers
{
    public class SarsReportController : Controller
    {
        private readonly SarsReportService _reportService;
        private readonly SarsReportDocumentService _pdfService;
        private readonly ApplicationDBContext _context;

        public SarsReportController(ApplicationDBContext context)
        {
            _context = context;

            _reportService = new SarsReportService(context);
            _pdfService = new SarsReportDocumentService();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var reports = await _context.SarsReports
                .OrderByDescending(x => x.PayCycle)
                .Select(x => new
                {
                    x.SarsReportId,
                    x.PayCycle,
                    x.GeneratedAt
                })
                .ToListAsync();

            return View(reports);
        }

        [HttpGet]
        public IActionResult GenerateReport(DateTime payCycle)
        {
            var report = _context.SarsReports
                .FirstOrDefault(x =>
                    x.PayCycle.Year == payCycle.Year &&
                    x.PayCycle.Month == payCycle.Month);

            if (report == null)
            {
                return NotFound("SARS report has not been generated for this month.");
            }

            if (report.PdfContent == null ||
                report.PdfContent.Length == 0)
            {
                return NotFound("SARS report PDF was not found.");
            }

            string fileName =
                $"SARS_Report_{report.PayCycle:yyyy_MM}.pdf";

            return File(
                report.PdfContent,
                "application/pdf",
                fileName);
        }
    }
}