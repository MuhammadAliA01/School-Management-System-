using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;

namespace SchoolManagmentSystem.Controllers
{
    public class PayslipDistributionController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IEmailService _emailService;

        public PayslipDistributionController(
          ApplicationDBContext context,
          IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Schedule(DateTime scheduledDate, TimeSpan scheduledTime, List<int> staffIds)
        {
            if (staffIds == null || !staffIds.Any())
            {
                return BadRequest("No staff members selected.");
            }

            var scheduledDateTime = scheduledDate.Date + scheduledTime;

            var payslips = await _context.Payslips
                .Where(p => p.Status == PayslipStatus.Generated && staffIds.Contains(p.StaffId))
                .ToListAsync();

            if (!payslips.Any())
            {
                return BadRequest("No generated payslips are available for scheduling.");
            }

            int scheduledCount = 0;

            foreach (var payslip in payslips)
            {
                var alreadyDistributed = await _context.PayslipDistributions
                    .AnyAsync(d => d.PayslipId == payslip.PayslipId &&
                                   d.StaffId == payslip.StaffId &&
                                   d.Status == DistributionStatus.Distributed);

                if (alreadyDistributed)
                {
                    continue;
                }

                var staff = await _context.Staffs
                    .FirstOrDefaultAsync(s => s.id == payslip.StaffId);

                if (staff == null)
                {
                    continue;
                }

                var accessToken = Guid.NewGuid().ToString();

                var distribution = new PayslipDistribution
                {
                    PayslipId = payslip.PayslipId,
                    StaffId = staff.id,
                    ScheduledFor = scheduledDateTime,
                    AccessToken = accessToken,
                    Status = DistributionStatus.Pending
                };

                _context.PayslipDistributions.Add(distribution);
                scheduledCount++;
            }

            await _context.SaveChangesAsync();
            return Ok($"Successfully scheduled {scheduledCount} payslip(s) for {scheduledDateTime:dd MMMM yyyy HH:mm}.");
        }

        [HttpPost]
        public async Task <IActionResult> SelectStaff(DateTime periodStart, DateTime periodEnd, List<int> staffIds)
        {
            if (staffIds == null || !staffIds.Any())
            {
                TempData["Error"] = "Please select at least one staff member.";
                // Handle case where no staff members were selected
                return RedirectToAction("SelectStaffForPayslips", "Payslip");
            }
            var selectedStaff = await _context.Staffs
        .Where(s => staffIds.Contains(s.id))
        .OrderBy(s => s.FirstName)
        .ToListAsync();

            ViewBag.PeriodStart = periodStart;
            ViewBag.PeriodEnd = periodEnd;
            ViewBag.StaffIds = staffIds;

            return View("Index", selectedStaff);
        }

        [HttpPost]
        public async Task<IActionResult> DistributePayslips(List<int> staffIds)
        {
            if (staffIds == null || !staffIds.Any())
            {
                return BadRequest("No staff members selected.");
            }

            var payslips = await _context.Payslips
                .Where(p => p.Status == PayslipStatus.Generated && staffIds.Contains(p.StaffId))
                .ToListAsync();

            if (!payslips.Any())
            {
                return BadRequest("No generated payslips are available for the selected staff.");
            }

            foreach (var payslip in payslips)
            {
                var alreadyDistributed = await _context.PayslipDistributions
                    .AnyAsync(d => d.PayslipId == payslip.PayslipId &&
                                   d.StaffId == payslip.StaffId &&
                                   d.Status == DistributionStatus.Distributed);

                if (alreadyDistributed)
                {
                    continue;
                }

                var staff = await _context.Staffs
                    .FirstOrDefaultAsync(s => s.id == payslip.StaffId);

                if (staff == null)
                {
                    continue;
                }

                var accessToken = Guid.NewGuid().ToString();

                var payslipLink = Url.Action(
                    "ViewPayslip",
                    "PayslipDistribution",
                    new { token = accessToken },
                    Request.Scheme);

                var distribution = new PayslipDistribution
                {
                    PayslipId = payslip.PayslipId,
                    StaffId = staff.id,
                    ScheduledFor = DateTime.Now,
                    AccessToken = accessToken,
                    Status = DistributionStatus.Pending
                };

                _context.PayslipDistributions.Add(distribution);

                try
                {
                    var emailSubject = "Your Payslip - Hybrid Learning Academy";
                    var emailBody = $@"
                <p>Dear {staff.FirstName} {staff.Surname},</p>
                <p>Your payslip is now available.</p>
                <p><a href=""{payslipLink}"">View Your Payslip</a></p>
                <p>You can view your payslip using the link above. If you are not currently logged in, you will be asked to log in first.</p>
                <p>Regards,<br>Hybrid Learning Academy</p>";

                    await _emailService.SendEmailAsync(
    staff.Email,
    emailSubject,
    emailBody,
    payslip.PdfContent,
    $"Payslip_{staff.FirstName}_{staff.Surname}_{payslip.PayPeriodStart:yyyyMM}.pdf");

                    distribution.DistributedAt = DateTime.Now;
                    distribution.Status = DistributionStatus.Distributed;
                }
                catch (Exception ex)
                {
                    distribution.Status = DistributionStatus.Failed;
                    distribution.FailureReason = ex.Message;
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Payslip distribution process completed.");
        }


[HttpGet]
public IActionResult ViewPayslip(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return NotFound("Invalid payslip link.");
            }

            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        returnUrl = $"/PayslipDistribution/ViewPayslip?token={token}"
                    });
            }

            return RedirectToAction(
                "OpenPayslip",
                new
                {
                    token = token
                });
        }

[HttpGet]
public async Task<IActionResult> OpenPayslip(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return NotFound("Invalid payslip link.");
            }

            var distribution = await _context.PayslipDistributions
                .FirstOrDefaultAsync(d =>
                    d.AccessToken == token &&
                    d.Status == DistributionStatus.Distributed);

            if (distribution == null)
            {
                return NotFound(
                    "Payslip link is invalid or has not been distributed.");
            }


            var payslip = await _context.Payslips
                .FirstOrDefaultAsync(p =>
                    p.PayslipId == distribution.PayslipId);

            if (payslip == null)
            {
                return NotFound("Payslip could not be found.");
            }

            if (payslip.PdfContent == null ||
                payslip.PdfContent.Length == 0)
            {
                return NotFound("Payslip PDF could not be found.");
            }

            return File(
                payslip.PdfContent,
                "application/pdf");
        }



        
    }
}