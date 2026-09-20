using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class EmployeeEarnings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int EmployeeID { get; set; }
        [ForeignKey("EmployeeID")]
        public virtual Staff staff { get; set; }

        public double salaryAmt { get; set; }
        public double taxRate { get; set; }
        public double pensionDeduction { get; set; }
        public double taxableIncome { get; set; }
        public double payeDeduction { get; set; }
        public double uifDeduction { get; set; }
        public double totalDeductions { get; set; }
        public double netSalary { get; set; }
        public DateTime payCycle { get; set; }


    }
}
