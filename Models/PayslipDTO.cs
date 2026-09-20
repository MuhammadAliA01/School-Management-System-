

namespace SchoolManagmentSystem.Models
{
    public class PayslipDTO
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string Role { get; set; }
        public DateTime PayPeriodStart { get; set; }
        public DateTime PayPeriodEnd { get; set; }

        public decimal BasePay { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal OvertimePay { get; set; }
        public decimal GrossPay { get; set; }

        public decimal PAYE { get; set; }
        public decimal UIF { get; set; }
        public decimal Pension { get; set; }
        public decimal MedicalAid { get; set; }

        public decimal NetPay { get; set; }

        //playing around with smt here
        public string IDNumber { get; set; }
    }
}
