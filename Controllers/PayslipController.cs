using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;


namespace SchoolManagmentSystem.Controllers
{
    public class PayslipController : Controller
    {
        private readonly PayslipGenerationService _payslipGenerationService;
        private readonly ApplicationDBContext _context;

        public PayslipController(PayslipGenerationService payslipGenerationService, ApplicationDBContext context)
        {
            _payslipGenerationService = payslipGenerationService;
            _context = context;
        }

        // Batch — all staff for a period
        [HttpPost]
        public IActionResult StageBatch(DateTime periodStart, DateTime periodEnd)
        {
            var staffList = _context.Staffs.ToList();
            var results = new List<object>();

            foreach (var staff in staffList)
            {
                try
                {
                    var data = _payslipGenerationService.BuildPayslipDataFromEarnings(staff, periodStart, periodEnd); var payslip = _payslipGenerationService.StagePayslip(data);
                    results.Add(new { staff.id, Status = "Success", payslip.PayslipId });
                }
                catch (Exception ex)
                {
                    results.Add(new { staff.id, Status = "Failed", Reason = ex.Message });
                }
            }

            return Json(results);
        }

        [HttpPost]
        public IActionResult Stage(int staffId, DateTime periodStart, DateTime periodEnd, string? regenerationReason = null)
        {
            var staff = _context.Staffs.Find(staffId);
            if (staff == null)
                return NotFound("Staff member not found.");

            var data = _payslipGenerationService.BuildPayslipDataFromEarnings(staff, periodStart, periodEnd);
            try
            {
                var payslip = _payslipGenerationService.StagePayslip(data, regenerationReason);
                return Json(new { payslip.PayslipId, payslip.Status });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Download PDF
        public IActionResult Download(int payslipId)
        {
            var payslip = _context.Payslips.Find(payslipId);
            if (payslip == null || payslip.PdfContent == null)
                return NotFound();

            return File(payslip.PdfContent, "application/pdf", $"payslip_{payslip.StaffId}_{payslip.PayPeriodStart:yyyyMM}_v{payslip.CurrentVersion}.pdf");
            // logic for incrementing version found in gen service cs. ref to stage payslip method for more details
        }

        // once Distribution confirms this will be called to finalize the payslip
        [HttpPost]
        public IActionResult Finalize(int payslipId)
        {
            _payslipGenerationService.FinalizePayslip(payslipId);
            return Json(new { Success = true, payslipId });
        }

        [HttpGet]
        public IActionResult History(int staffId)
        {
            var history = _context.Payslips
                .Where(p => p.StaffId == staffId)
                .OrderByDescending(p => p.PayPeriodStart)
                .Select(p => new
                {
                    p.PayslipId,
                    p.PayPeriodStart,
                    p.PayPeriodEnd,
                    p.Status,
                    p.CurrentVersion
                })
                .ToList();

            return Json(history);
        }
        [HttpGet]
        public IActionResult GetStaffList()
        {
            var staff = _context.Staffs
                .Select(s => new { s.id, Name = s.FirstName + " " + s.Surname })
                .ToList();
            return Json(staff);
        }
        [HttpPost]
        public IActionResult Regenerate(int payslipId, string regenerationReason)
        {
            if (string.IsNullOrWhiteSpace(regenerationReason))
                return BadRequest("A reason is required to regenerate a payslip.");

            var existing = _context.Payslips.Find(payslipId);
            if (existing == null)
                return NotFound("Payslip not found.");

            var staff = _context.Staffs.Find(existing.StaffId);
            if (staff == null)
                return NotFound("Staff member not found.");

            var data = _payslipGenerationService.BuildPayslipDataFromEarnings(staff, existing.PayPeriodStart, existing.PayPeriodEnd);
            var payslip = _payslipGenerationService.StagePayslip(data, regenerationReason);
            return Json(new { payslip.PayslipId, payslip.Status, payslip.CurrentVersion });
        }

        [HttpGet]
        public async Task<IActionResult> SelectStaffForPayslips()
        {
            var staff = await _context.Staffs
                        .OrderBy(x => x.FirstName)
                        .ToListAsync();

            return View(staff);
        }

        [HttpGet]
        public IActionResult RegeneratePayslip()
        {
            return View();
        }
    }
}