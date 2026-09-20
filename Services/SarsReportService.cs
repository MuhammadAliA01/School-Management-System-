using SchoolManagmentSystem.Models;
namespace SchoolManagmentSystem.Services
{
    public class SarsReportService
    {
        private readonly ISarsTaxDataProvider _taxDataProvider;
        private const decimal SdlAnnualThreshold = 500000m;

        public SarsReportService(ISarsTaxDataProvider taxDataProvider)
        {
            _taxDataProvider = taxDataProvider;
        }

        public SarsMonthlyReportDTO GenerateReport(List<int> staffIds, string payPeriod)
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
    }
}
