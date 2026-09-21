using SchoolManagmentSystem.Models;
namespace SchoolManagmentSystem.Services
{
    public class SarsReportService
    {
        private readonly ApplicationDBContext _context;
        private const decimal SdlAnnualThreshold = 500000m;

        public SarsReportService(ApplicationDBContext context)
        {
            _context = context;
        }

        public SarsMonthlyReportDTO GenerateReport(DateTime payCycle)
        {
            var earnings = _context.EmployeeEarnings
                .Where(e => e.payCycle.Month == payCycle.Month && e.payCycle.Year == payCycle.Year)
                .ToList();

            var report = new SarsMonthlyReportDTO
            {
                PayPeriod = payCycle,
                EmployeeCount = earnings.Count,
                TotalPAYE = (decimal)earnings.Sum(e => e.payeDeduction),
                TotalEmployeeUIF = (decimal)earnings.Sum(e => e.uifDeduction),
                TotalGrossRemuneration = (decimal)earnings.Sum(e => e.salaryAmt)
            };

            report.TotalEmployerUIF = report.TotalEmployeeUIF; // employer matches employee UIF, same cap
            report.SdlThresholdMet = (report.TotalGrossRemuneration * 12) > SdlAnnualThreshold;
            if (report.SdlThresholdMet)
            {
                report.TotalSDL = report.TotalGrossRemuneration * 0.01m;
            }
            else
            {
                report.TotalSDL = 0m;
            }
            report.GrandTotalPayableToSars = report.TotalPAYE + report.TotalEmployeeUIF + report.TotalEmployerUIF + report.TotalSDL;

            return report;
        }
    }
}
