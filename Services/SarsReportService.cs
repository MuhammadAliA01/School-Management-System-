using SchoolManagmentSystem.Models;
namespace SchoolManagmentSystem.Services
{
    public class SarsReportService
    {
        private readonly ISarsTaxDataProvider _taxDataProvider;
        private const decimal SdlAnnualThreshold = 500000m;
        private readonly ApplicationDBContext _context;
        public SarsReportService(ISarsTaxDataProvider taxDataProvider)
        {
            _taxDataProvider = taxDataProvider;
        }

        public SarsMonthlyReportDTO GenerateReport(List<int> staffIds, DateTime payPeriod)
        {
            var report = new SarsMonthlyReportDTO { PayPeriod = payPeriod, EmployeeCount = staffIds.Count };

            foreach (var id in staffIds)
            {
                var contribution = _taxDataProvider.GetContribution(id, payPeriod);
                report.TotalPAYE += contribution.PAYE;
                report.TotalEmployeeUIF += contribution.EmployeeUIF;
                report.TotalEmployerUIF += contribution.EmployerUIF;
                report.TotalGrossRemuneration += contribution.GrossRemuneration;
            }

            report.SdlThresholdMet = (report.TotalGrossRemuneration * 12) > SdlAnnualThreshold;
            report.TotalSDL = report.SdlThresholdMet ? report.TotalGrossRemuneration * 0.01m : 0m;
            report.GrandTotalPayableToSars = report.TotalPAYE + report.TotalEmployeeUIF + report.TotalEmployerUIF + report.TotalSDL;

            return report;
        }

        public SarsReportService(ISarsTaxDataProvider taxDataProvider, ApplicationDBContext context)
        {
            _taxDataProvider = taxDataProvider;
            _context = context;
        }

        public List<int> GetStaffIdsForPeriod(DateTime payCycle)
        {
            return _context.EmployeeEarnings
                .Where(e => e.payCycle.Month == payCycle.Month && e.payCycle.Year == payCycle.Year)
                .Select(e => e.EmployeeID)
                .Distinct()
                .ToList();
        }
    }
}
