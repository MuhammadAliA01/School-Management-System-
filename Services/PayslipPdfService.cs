using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Services
{
    public class PayslipPdfService
    {
        public byte[] BuildPdf(PayslipDTO data)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.Header().Text("Payslip").FontSize(20).Bold();

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Staff: {data.StaffName} ({data.Role})");
                        col.Item().Text($"Period: {data.PayPeriodStart:d} - {data.PayPeriodEnd:d}");
                        col.Item().PaddingTop(10).Text("Earnings").Bold();
                        col.Item().Text($"Base Pay: R{data.BasePay:N2}");
                        col.Item().Text($"Overtime: R{data.OvertimePay:N2}");
                        col.Item().Text($"Gross Pay: R{data.GrossPay:N2}");

                        col.Item().PaddingTop(10).Text("Deductions").Bold();
                        col.Item().Text($"PAYE: R{data.PAYE:N2}");
                        col.Item().Text($"UIF: R{data.UIF:N2}");
                        col.Item().Text($"Pension: R{data.Pension:N2}");
                        col.Item().Text($"Medical Aid: R{data.MedicalAid:N2}");

                        col.Item().PaddingTop(10).Text($"Net Pay: R{data.NetPay:N2}").Bold().FontSize(14);
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}