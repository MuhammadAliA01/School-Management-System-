using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Services
{
    public interface ISarsTaxDataProvider
    {
        SarsTaxContributionDTO GetContribution(int staffId, DateTime payPeriod);
    }
}
