using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;
using Stripe.V2;
using System.Net.Http.Headers;
namespace SchoolManagmentSystem.Controllers
{

    
    public class PayrollController : Controller
    {

        private readonly ApplicationDBContext _context;
        private readonly ModernTreasuryService _modernTreasuryService;
        private readonly HttpClient _httpClient;
        private readonly IEmailService _emailService;
        private readonly PayslipGenerationService _payslipGenerationService;


        public PayrollController(ApplicationDBContext context, ModernTreasuryService modernTreasuryService, HttpClient httpClient, IEmailService emailService,
    PayslipGenerationService payslipGenerationService)
        {
            _context = context;
            _modernTreasuryService = modernTreasuryService;
            _httpClient = httpClient;
            _emailService = emailService;
            _payslipGenerationService = payslipGenerationService;
        }

        

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> PayrollDashboard()
        {

            var empList = await _context.Staffs.ToListAsync();


            foreach (var id in empList)
            {
                var empID = id.id;
                var salary = 0.0;

                var hoursWorked = _context.StaffAttendances.Where(x => x.StaffId == id.id).Select(x => x.TotalHoursWorked).FirstOrDefault();

                if (id.Role == "Admin")
                {
                    salary = 30000;
                }
                else if (id.Role == "Teacher")
                {
                    salary = 40000;

                }
                else if (id.Role == "Principal")
                {
                    salary = 50000;

                }
                else if (id.Role == "Cleaner")
                {
                    salary = 40 * Convert.ToDouble(hoursWorked);

                }
                
                double pension = salary * 0.075;
                double annualTaxableIncome = (salary - pension) * 12.0;

                
                double appliedRate = 0.18;
                double baseTax = 0;
                double minIncome = 1;

                
                var taxBrackets = _context.TaxBrackets
                    .Where(b => b.TaxYear == "2026/2027")
                    .OrderBy(b => b.MinIncome)
                    .ToList();

                var matchedBracket = taxBrackets.FirstOrDefault(b => annualTaxableIncome <= b.MaxIncome) ?? taxBrackets.LastOrDefault();

                if (matchedBracket != null)
                {
                    appliedRate = matchedBracket.TaxRate;
                    baseTax = matchedBracket.BaseTaxAmount;
                    minIncome = matchedBracket.MinIncome;
                }

                double taxableAboveBracket = Math.Max(0, annualTaxableIncome - minIncome);
                double annualPaye = baseTax + (taxableAboveBracket * appliedRate);

                
                annualPaye -= 17235;

               
                annualPaye = Math.Max(0, annualPaye);

                double monthlyPaye = annualPaye / 12.0;

                double monthlyUif = Math.Min(salary * 0.01, 177.12);
                double totalDeductions = pension + monthlyPaye + monthlyUif;
                double finalNet = salary - totalDeductions;
                

                var exists = _context.EmployeeEarnings.Any(x => x.EmployeeID == id.id && x.payCycle.Month == DateTime.Now.Month && x.payCycle.Year == DateTime.Now.Year);

                if (!exists)
                {
                    var newEntry = new EmployeeEarnings
                    {
                        EmployeeID = empID,
                        salaryAmt = salary,
                        payCycle = DateTime.Now,

                        //  new fields
                        pensionDeduction = Math.Round(pension, 2),
                        taxableIncome = Math.Round(annualTaxableIncome, 2),
                        payeDeduction = Math.Round(monthlyPaye, 2),
                        uifDeduction = Math.Round(monthlyUif, 2),
                        totalDeductions = Math.Round(totalDeductions, 2),
                        netSalary = Math.Round(finalNet, 2),
                        taxRate = appliedRate * 100.0
                    };

                    _context.EmployeeEarnings.Add(newEntry);

                }


            }

            _context.SaveChanges();

            var listOfEarnings = await _context.EmployeeEarnings.ToListAsync();

            return View(listOfEarnings);
        }

        [HttpGet]
        public async Task<IActionResult> GetDeductionsBreakdown(int staffId)
        {
            var earning = await _context.EmployeeEarnings
                .Include(e => e.staff)
                .FirstOrDefaultAsync(x => x.EmployeeID == staffId && x.payCycle.Month == DateTime.Now.Month && x.payCycle.Year == DateTime.Now.Year);

            if (earning == null) return NotFound();

            var taxBrackets = await _context.TaxBrackets.Where(b => b.TaxYear == "2026/2027").OrderBy(b => b.MinIncome).ToListAsync();
            var matchedBracket = taxBrackets.FirstOrDefault(b => earning.taxableIncome <= b.MaxIncome) ?? taxBrackets.LastOrDefault();

            // Calculate age and rebates
            int age = GetAgeFromSouthAfricanId(earning.staff.IDNumber);
            double annualRebate = 17235.0; // Primary
            string rebateType = "Primary Rebate (<65)";

            if (age >= 75)
            {
                annualRebate += 9444.0 + 3145.0;
                rebateType = "Primary + Sec + Tert (75+)";
            }
            else if (age >= 65)
            {
                annualRebate += 9444.0;
                rebateType = "Primary + Secondary (65+)";
            }

            double monthlyRebateDiscount = annualRebate / 12.0;

            
            double medicalTaxCredit = 0.0;

            double taxBeforeRebate = earning.payeDeduction + monthlyRebateDiscount + medicalTaxCredit;

            // Employer Contributions
            double employerUif = earning.uifDeduction; 
            double employerSdl = earning.salaryAmt * 0.01; 

            return Json(new
            {
                fullName = $"{earning.staff.FirstName} {earning.staff.Surname}",
                role = earning.staff.Role,
                age = age,
                gross = earning.salaryAmt,
                pension = earning.pensionDeduction,
                taxable = earning.taxableIncome,

                // Bracket Info
                bracketMin = matchedBracket?.MinIncome ?? 0,
                bracketMax = matchedBracket?.MaxIncome ?? 0,
                taxRate = earning.taxRate,

                // Tax Breakdown
                taxBeforeRebate = taxBeforeRebate,
                rebateType = rebateType,
                monthlyRebate = monthlyRebateDiscount,
                medicalCredit = medicalTaxCredit,
                paye = earning.payeDeduction,
                uif = earning.uifDeduction,
                totalDeductions = earning.totalDeductions,
                net = earning.netSalary,

                // Employer Side
                empUif = employerUif,
                empSdl = employerSdl
            });
        }

        private int GetAgeFromSouthAfricanId(string idNumber)
        {
            if (string.IsNullOrWhiteSpace(idNumber) || idNumber.Length < 6)
            {
                return 35; // Default  if ID is missing or invalid
            }

            try
            {
                int yearPrefix = int.Parse(idNumber.Substring(0, 2));
                int currentTwoDigitYear = DateTime.Now.Year % 100;
                int fullYear = (yearPrefix <= currentTwoDigitYear) ? (2000 + yearPrefix) : (1900 + yearPrefix);
                int month = int.Parse(idNumber.Substring(2, 2));
                int day = int.Parse(idNumber.Substring(4, 2));

                DateTime birthDate = new DateTime(fullYear, month, day);
                int age = DateTime.Now.Year - birthDate.Year;
                if (DateTime.Now.DayOfYear < birthDate.DayOfYear) age--;
                return age;
            }
            catch
            {
                return 35; 
            }
        }
    

        public async Task<IActionResult> Transfer()
        {

            foreach(var id in _context.BankAccounts)
            {
                var bankNum = id.accountNumber;


                var result =
              await _modernTreasuryService.TransferAsync(
                  "01a0a0f2-20db-78b2-8ab8-5d09e1f5e27e", //to account
                  "01a0c052-ec58-7399-a0f4-932ece681b95", //from account
                  1000000,
                  $"Transfer from sdfsd to sdfsdf");
            }


            return RedirectToAction("HomePage", "Home");

        }

 

        [HttpPost]
        public async Task<IActionResult> ConfirmDistribution(
    List<int> staffIds,
    bool isScheduled,
    DateTime? scheduledDate,
    TimeSpan? scheduledTime)
        {
            if (staffIds == null || !staffIds.Any())
            {
                TempData["Error"] = "No staff members selected.";

                return RedirectToAction(
                    "SelectStaffForPayslips",
                    "Payslip");
            }

            if (isScheduled)
            {
                if (!scheduledDate.HasValue || !scheduledTime.HasValue)
                {
                    TempData["Error"] = "Please select a date and time.";

                    return RedirectToAction(
                        "Index",
                        "PayslipDistribution");
                }
            }

            var selectedStaff = await _context.Staffs
                .Where(s => staffIds.Contains(s.id))
                .OrderBy(s => s.FirstName)
                .ToListAsync();

            if (!selectedStaff.Any())
            {
                TempData["Error"] = "The selected employees could not be found.";

                return RedirectToAction(
                    "SelectStaffForPayslips",
                    "Payslip");
            }

            ViewBag.StaffIds = staffIds;
            ViewBag.IsScheduled = isScheduled;
            ViewBag.ScheduledDate = scheduledDate;
            ViewBag.ScheduledTime = scheduledTime;

            return View("ConfirmDistribution", selectedStaff);
        }



        [HttpPost]
        public async Task<IActionResult> ProcessDistribution(
    List<int> staffIds,
    bool isScheduled,
    DateTime? scheduledDate,
    TimeSpan? scheduledTime)
        {
            if (staffIds == null || !staffIds.Any())
            {
                TempData["Error"] = "No staff members were selected.";

                return RedirectToAction(
                    "SelectStaffForPayslips",
                    "Payslip");
            }

            if (isScheduled)
            {
                if (!scheduledDate.HasValue || !scheduledTime.HasValue)
                {
                    TempData["Error"] = "Please select a scheduled date and time.";

                    return RedirectToAction(
                        "Index",
                        "PayslipDistribution");
                }
            }



            var selectedStaff = await _context.Staffs
                .Where(s => staffIds.Contains(s.id))
                .OrderBy(s => s.FirstName)
                .ToListAsync();

            if (!selectedStaff.Any())
            {
                TempData["Error"] = "No selected employees were found.";

                return RedirectToAction(
                    "SelectStaffForPayslips",
                    "Payslip");
            }




            foreach (var staff in selectedStaff)
            {
                try
                {
         
                    DateTime periodStart = new DateTime(
                        DateTime.Now.Year,
                        DateTime.Now.Month,
                        1);

                    DateTime periodEnd = periodStart
                        .AddMonths(1)
                        .AddDays(-1);



                    var payslipData =
                        _payslipGenerationService
                            .BuildPayslipDataFromEarnings(
                                staff,
                                periodStart,
                                periodEnd);




                    var payslip =
                        _payslipGenerationService
                            .StagePayslip(payslipData);



                    if (payslip.PdfContent == null ||
                        payslip.PdfContent.Length == 0)
                    {
                        continue;
                    }




                    var bankAccount = await _context.BankAccounts
                        .FirstOrDefaultAsync(x =>
                            x.employeeID == staff.id);

                    if (bankAccount == null)
                    {
                        continue;
                    }



                    var earnings = await _context.EmployeeEarnings
                        .FirstOrDefaultAsync(x =>
                            x.EmployeeID == staff.id &&
                            x.payCycle.Month == DateTime.Now.Month &&
                            x.payCycle.Year == DateTime.Now.Year);

                    if (earnings == null)
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
                        AccessToken = accessToken,
                        Status = DistributionStatus.Pending
                    };



                    if (isScheduled)
                    {
                        distribution.ScheduledFor =
                            scheduledDate.Value.Date +
                            scheduledTime.Value;

                        _context.PayslipDistributions.Add(distribution);

                        continue;
                    }


                    distribution.ScheduledFor = DateTime.Now;

                    _context.PayslipDistributions.Add(distribution);



                    var emailSubject =
                        "Your Payslip - Hybrid Learning Academy";

                    var emailBody = $@"
                <p>Dear {staff.FirstName} {staff.Surname},</p>

                <p>Your payslip has been generated and is now available.</p>

                <p>
                    Your payslip is attached to this email as a PDF.
                </p>

                <p>
                    You can also view your payslip online:
                </p>

                <p>
                    <a href=""{payslipLink}"">
                        View Your Payslip
                    </a>
                </p>

                <p>
                    Regards,<br>
                    Hybrid Learning Academy
                </p>";


                    await _emailService.SendEmailAsync(
                        staff.Email,
                        emailSubject,
                        emailBody,
                        payslip.PdfContent,
                        $"Payslip_{staff.FirstName}_{staff.Surname}_{payslip.PayPeriodStart:yyyyMM}.pdf");


                    distribution.DistributedAt = DateTime.Now;
                    distribution.Status = DistributionStatus.Distributed;


                    await _modernTreasuryService.TransferAsync(
                        bankAccount.accountNumber,
                        "01a0a0f2-20db-78b2-8ab8-5d09e1f5e27e",
                        (decimal)earnings.netSalary,
                        $"Salary payment for {staff.FirstName} {staff.Surname}");



                }
                catch (Exception ex)
                {
                    

                    TempData["Error"] =
                        $"An error occurred while processing " +
                        $"{staff.FirstName} {staff.Surname}: {ex.Message}";

                    continue;
                }
            }


            await _context.SaveChangesAsync();


            if (isScheduled)
            {
                TempData["Success"] =
                    $"Payroll distribution scheduled for " +
                    $"{scheduledDate.Value:dd MMMM yyyy} " +
                    $"{scheduledTime.Value:hh\\:mm}.";
            }
            else
            {
                TempData["Success"] =
                    "Payslips generated, emailed and processed successfully.";
            }


            return RedirectToAction(
                "HomePage",
                "Home");
        }



    }
}
  
