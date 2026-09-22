using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Services
{
    public class SarsReportDocumentService
    {
        public byte[] GeneratePdf(SarsMonthlyReportDTO report)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // Header
                    page.Header().Column(col =>
                    {
                        col.Item().Text("SARS Monthly Payment Report").FontSize(18).Bold();
                        col.Item().Text($"Pay Period: {report.PayPeriod:MMMM yyyy}");
                        col.Item().Text($"Employees Included: {report.EmployeeCount}");
                        col.Item().PaddingTop(5).LineHorizontal(1);
                    });

                    // Body
                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Spacing(15);

                        // PAYE & UIF section
                        col.Item().Border(1).Padding(10).Column(section =>
                        {
                            section.Item().Text("Tax & UIF Contributions").Bold().FontSize(13);
                            section.Item().PaddingTop(5).Row(row =>
                            {
                                row.RelativeItem().Text("Total PAYE");
                                row.RelativeItem().AlignRight().Text($"R {report.TotalPAYE:N2}");
                            });
                            section.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Total Employee UIF");
                                row.RelativeItem().AlignRight().Text($"R {report.TotalEmployeeUIF:N2}");
                            });
                            section.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Total Employer UIF");
                                row.RelativeItem().AlignRight().Text($"R {report.TotalEmployerUIF:N2}");
                            });
                        });

                        // SDL 
                        col.Item().Border(1).Padding(10).Column(section =>
                        {
                            section.Item().Text("Skills Development Levy (SDL)").Bold().FontSize(13);
                            section.Item().PaddingTop(5).Row(row =>
                            {
                                row.RelativeItem().Text("Total Gross Remuneration");
                                row.RelativeItem().AlignRight().Text($"R {report.TotalGrossRemuneration:N2}");
                            });
                            section.Item().Row(row =>
                            {
                                row.RelativeItem().Text("SDL Threshold Met (>R500,000 annually)");
                                row.RelativeItem().AlignRight().Text(report.SdlThresholdMet ? "Yes" : "No");
                            });
                            section.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Total SDL Payable");
                                row.RelativeItem().AlignRight().Text($"R {report.TotalSDL:N2}");
                            });
                        });

                        // toootalsss
                        col.Item().Background(Colors.Grey.Lighten3).Border(1).Padding(12).Row(row =>
                        {
                            row.RelativeItem().Text("Grand Total Payable to SARS").Bold().FontSize(14);
                            row.RelativeItem().AlignRight().Text($"R {report.GrandTotalPayableToSars:N2}").Bold().FontSize(14);
                        });
                    });

                    // Footer
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated on ");
                        x.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm"));
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}