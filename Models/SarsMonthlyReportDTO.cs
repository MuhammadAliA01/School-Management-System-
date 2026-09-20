namespace SchoolManagmentSystem.Models
{
    public class SarsMonthlyReportDTO
    {
        public string PayPeriod { get; set; }
        public int EmployeeCount { get; set; }
        public decimal TotalPAYE { get; set; }
        public decimal TotalEmployeeUIF { get; set; }
        public decimal TotalEmployerUIF { get; set; }
        public decimal TotalGrossRemuneration { get; set; }
        public bool SdlThresholdMet { get; set; }
        public decimal TotalSDL { get; set; }
        public decimal GrandTotalPayableToSars { get; set; }
    }
}
