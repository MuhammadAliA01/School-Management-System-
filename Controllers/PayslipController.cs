using Microsoft.AspNetCore.Mvc;
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
                    var data = BuildDummyPayslipData(staff, periodStart, periodEnd);
                    var payslip = _payslipGenerationService.StagePayslip(data);
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

            var data = BuildDummyPayslipData(staff, periodStart, periodEnd);

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

            return File(payslip.PdfContent, "application/pdf", $"payslip_{payslip.StaffId}_{payslip.PayPeriodStart:yyyyMM}.pdf");
        }

        // once Distribution confirms this will be called to finalize the payslip
        [HttpPost]
        public IActionResult Finalize(int payslipId)
        {
            _payslipGenerationService.FinalizePayslip(payslipId);
            return Json(new { Success = true, payslipId });
        }

        //temporary data for demonstration of test; in a real scenario, this would be calculated based on actual payroll data
        private PayslipDTO BuildDummyPayslipData(Staff staff, DateTime periodStart, DateTime periodEnd)
        {
            return new PayslipDTO
            {
                StaffId = staff.id,
                StaffName = $"{staff.FirstName} {staff.Surname}",
                Role = staff.Role,
                IDNumber = staff.IDNumber,
                PayPeriodStart = periodStart,
                PayPeriodEnd = periodEnd,
                BasePay = 15000,
                HoursWorked = 160,
                OvertimePay = 0,
                GrossPay = 15000,
                PAYE = 2000,
                UIF = 150,
                Pension = 750,
                MedicalAid = 500,
                NetPay = 11600
            };
        }
    }
}