using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class BankAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int employeeID { get; set; }
        [ForeignKey("employeeID")]
        public virtual Staff staff { get; set; }
        public string accountNumber { get; set; }
        public string accountName { get; set; }
    }
}
