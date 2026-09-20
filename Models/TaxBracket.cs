using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class TaxBracket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string TaxYear { get; set; } = "2026/2027";

        public double MinIncome { get; set; }
        public double MaxIncome { get; set; }
        public double BaseTaxAmount { get; set; }
        public double TaxRate { get; set; }
    }
}