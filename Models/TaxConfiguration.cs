using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class TaxConfiguration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string TaxYear { get; set; } = "2026/2027";

        public double PrimaryRebate { get; set; } = 17235.0;
        public double SecondaryRebate { get; set; } = 9444.0;
        public double TertiaryRebate { get; set; } = 3145.0;

        public double PensionContributionRate { get; set; } = 0.075;
        public double UifRate { get; set; } = 0.01;
        public double UifMonthlyCap { get; set; } = 177.12;
    }
}
