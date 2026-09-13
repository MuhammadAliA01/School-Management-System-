using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class Transactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int accTransferredTo { get; set; }
        [ForeignKey("accTransferredTo")]
        public virtual BankAccount bAccount { get; set; }
        public double amtTransferred { get; set; }
        public DateTime transferDate { get; set; }
    }
}
