using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Services
{
    public class PayslipPdfService
    {
        private readonly IWebHostEnvironment _env;

        public PayslipPdfService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public byte[] BuildPdf(PayslipDTO data)
        {
            var logoPath = Path.Combine(_env.WebRootPath, "images", "hla-logo.jfif");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));

                   
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.ConstantItem(160).Image(logoPath).FitWidth();

                            row.RelativeItem().PaddingLeft(10).AlignMiddle().Column(c =>
                            {
                                c.Item().Text("Payslip").FontSize(13).Bold();
                            });

                            row.ConstantItem(150).AlignRight().Column(c =>
                            {
                                c.Item().Text($"Period: {data.PayPeriodStart:MMM yyyy}").FontSize(10);
                                c.Item().Text($"{data.PayPeriodStart:dd MMM} - {data.PayPeriodEnd:dd MMM yyyy}").FontSize(9).FontColor(Colors.Grey.Darken1);
                            });
                        });

                        col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Darken1);
                    });

                   
                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        // Employer details block
                        col.Item().PaddingBottom(10).Column(c =>
                        {
                            c.Item().Text("Hybrid Learning Academy (HLA)").FontSize(11).Bold();
                            c.Item().Text("51 Tourmalin Ave, Chatsworth, Durban, 4092").FontSize(9).FontColor(Colors.Grey.Darken1);
                        });

                        col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);

                        // Staff information block
                        col.Item().PaddingTop(10).PaddingBottom(12).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Staff member").FontSize(9).FontColor(Colors.Grey.Darken1);
                                c.Item().Text(data.StaffName).FontSize(12).Bold();
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Role").FontSize(9).FontColor(Colors.Grey.Darken1);
                                c.Item().Text(data.Role).FontSize(12).Bold();
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Employee code").FontSize(9).FontColor(Colors.Grey.Darken1);
                                c.Item().Text($"#{data.StaffId}").FontSize(12).Bold();
                            });
                        });

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Darken1);

                        // Earnings 
                        col.Item().PaddingTop(12).Element(container => BuildSectionBlock(container, "Earnings", table =>
                        {
                            AddRow(table, "Base pay", data.BasePay);
                            AddRow(table, "Hours worked", data.HoursWorked, isCurrency: false);
                            AddRow(table, "Overtime pay", data.OvertimePay);
                            AddRow(table, "Gross pay", data.GrossPay, bold: true);
                        }));

                        // Deductions 
                        col.Item().PaddingTop(16).Element(container => BuildSectionBlock(container, "Deductions", table =>
                        {
                            AddRow(table, "PAYE", data.PAYE);
                            AddRow(table, "UIF", data.UIF);
                            AddRow(table, "Pension fund", data.Pension);
                            AddRow(table, "Medical aid", data.MedicalAid);
                        }));

                        // Net pay 
                        col.Item().PaddingTop(20).Border(1).BorderColor(Colors.Grey.Darken1)
                            .Background(Colors.Grey.Lighten3).Padding(12).Row(row =>
                            {
                                row.RelativeItem().AlignMiddle().Text("Net pay").FontSize(14).Bold();
                                row.RelativeItem().AlignRight().AlignMiddle().Text($"R{data.NetPay:N2}").FontSize(16).Bold();
                            });
                    });

                    
                    page.Footer().PaddingTop(10).Column(col =>
                    {
                        col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                        col.Item().PaddingTop(6).Row(row =>
                        {
                            row.RelativeItem().Text($"Generated {DateTime.Now:dd MMM yyyy}").FontSize(8).FontColor(Colors.Grey.Darken1);
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        // Adding boarders for ux
        private void BuildSectionBlock(IContainer container, string title, Action<TableDescriptor> buildRows)
        {
            container.Border(1).BorderColor(Colors.Grey.Lighten1).Column(col =>
            {
                col.Item().Background(Colors.Grey.Lighten3).Padding(6).Text(title).FontSize(12).Bold();

                col.Item().Padding(10).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3);
                        c.RelativeColumn(2);
                    });

                    buildRows(table);
                });
            });
        }

        private void AddRow(TableDescriptor table, string label, decimal value, bool isCurrency = true, bool bold = false)
        {
            var labelCell = table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).Text(label);
            var valueCell = table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).AlignRight().Text(isCurrency ? $"R{value:N2}" : $"{value:N1}");

            if (bold)
            {
                labelCell.Bold();
                valueCell.Bold();
            }
        }
    }
}