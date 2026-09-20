using SchoolManagmentSystem.Services;
namespace SchoolManagmentSystem.Models
{
    public class MockSarsTaxDataProvider : ISarsTaxDataProvider
    {
        public SarsTaxContributionDTO GetContribution(int staffId, DateTime payPeriod)
        {
            // Tempo values for now
            return new SarsTaxContributionDTO
            {
                StaffId = staffId,
                StaffName = "Placeholder",
                PAYE = 5000m,
                EmployeeUIF = 177.12m,
                EmployerUIF = 177.12m,
                GrossRemuneration = 17712m
            };
        }
    }
}
