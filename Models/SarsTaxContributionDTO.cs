namespace SchoolManagmentSystem.Models
{
    public class SarsTaxContributionDTO
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public decimal PAYE { get; set; }
        public decimal EmployeeUIF { get; set; }
        public decimal EmployerUIF { get; set; }
        public decimal GrossRemuneration { get; set; }
    }
}
