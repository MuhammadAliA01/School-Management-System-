using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers
{
    [ApiController]
    [Route("api/mobile/payslip-history")]
    public class MobilePayslipHistoryController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public MobilePayslipHistoryController(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET api/mobile/payslip-history?staffId=123
        /// Returns the specified employee's payslip history, most recent first.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PayslipHistoryItemDto>>> GetHistory([FromQuery] int staffId)
        {
            if (staffId <= 0)
                return BadRequest("A valid Staff ID must be provided.");

            var payslips = await _context.Payslips
                .AsNoTracking()
                .Where(p => p.StaffId == staffId)
                .OrderByDescending(p => p.PayPeriodStart)
                .Select(p => new PayslipHistoryItemDto
                {
                    PayslipId = p.PayslipId,
                    PayPeriodStart = p.PayPeriodStart,
                    PayPeriodEnd = p.PayPeriodEnd,
                    Status = p.Status.ToString(),
                    FailureReason = p.FailureReason,
                    CurrentVersion = p.CurrentVersion,
                    GeneratedAt = p.GeneratedAt
                })
                .ToListAsync();

            return Ok(payslips);
        }

        /// <summary>
        /// GET api/mobile/payslip-history/{payslipId}/download?staffId=123
        /// Streams the PDF for one payslip, matching the specified staff member.
        /// </summary>
        [HttpGet("{payslipId:int}/download")]
        public async Task<IActionResult> DownloadPayslip(int payslipId, [FromQuery] int staffId)
        {
            if (staffId <= 0)
                return BadRequest("A valid Staff ID must be provided.");

            var payslip = await _context.Payslips
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PayslipId == payslipId && p.StaffId == staffId);

            if (payslip == null)
                return NotFound("Payslip not found.");

            if (payslip.Status != PayslipStatus.Generated || payslip.PdfContent == null || payslip.PdfContent.Length == 0)
                return BadRequest("This payslip isn't ready to download yet.");

            var fileName = $"Payslip_{payslip.PayPeriodStart:yyyyMM}.pdf";
            return File(payslip.PdfContent, "application/pdf", fileName);
        }
    }
}