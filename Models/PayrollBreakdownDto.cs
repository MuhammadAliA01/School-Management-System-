namespace SchoolManagmentSystem.Models
{
    public class PayrollBreakdownDto
    {
        // Employee Info
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int Age { get; set; }

        // Income & Pre-Tax Deductions
        public double Gross { get; set; }
        public double Pension { get; set; }
        public double Taxable { get; set; }

        // Bracket Info
        public double BracketMin { get; set; }
        public double BracketMax { get; set; }
        public double TaxRate { get; set; }

        // Tax Breakdown
        public double TaxBeforeRebate { get; set; }
        public string RebateType { get; set; } = string.Empty;
        public double MonthlyRebate { get; set; }
        public double MedicalCredit { get; set; }

        // Final Deductions & Net
        public double Paye { get; set; }
        public double Uif { get; set; }
        public double TotalDeductions { get; set; }
        public double Net { get; set; }

        // Employer Side
        public double EmpUif { get; set; }
        public double EmpSdl { get; set; }
    }
}