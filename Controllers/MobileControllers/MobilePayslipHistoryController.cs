using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers
{
    [ApiController]
    [Route("api/mobile/payslips")]
    public class MobilePayslipHistoryController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public MobilePayslipHistoryController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET api/mobile/payslips/history/5
        // Returns only the list info (no PDF bytes) so it stays fast on mobile data
        [HttpGet("history/{staffId}")]
        public async Task<IActionResult> GetHistory(int staffId)
        {
            var payslips = await _context.Payslips
                .Where(p => p.StaffId == staffId && p.Status == PayslipStatus.Generated)
                .OrderByDescending(p => p.PayPeriodEnd)
                .Select(p => new
                {
                    p.PayslipId,
                    p.PayPeriodStart,
                    p.PayPeriodEnd,
                    p.GeneratedAt,
                    p.CurrentVersion
                })
                .ToListAsync();

            return Ok(payslips);
        }

        // GET api/mobile/payslips/download/12/5
        [HttpGet("download/{payslipId}/{staffId}")]
        public async Task<IActionResult> Download(int payslipId, int staffId)
        {
            // staffId check makes sure an employee can only download their own payslip
            var payslip = await _context.Payslips
                .FirstOrDefaultAsync(p => p.PayslipId == payslipId
                                       && p.StaffId == staffId
                                       && p.Status == PayslipStatus.Generated);

            if (payslip == null || payslip.PdfContent == null || payslip.PdfContent.Length == 0)
                return NotFound("Payslip not found.");

            return File(payslip.PdfContent, "application/pdf",
                        $"Payslip_{payslip.PayPeriodEnd:yyyy-MM}.pdf");
        }
    }
}