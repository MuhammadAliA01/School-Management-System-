using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System.Net;
using System.Net.Mail;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Services
{
    public class AttendanceReportService
    {
        private readonly ApplicationDBContext _context;

        public AttendanceReportService(ApplicationDBContext context)
        {
            _context = context;
        }

        public byte[] GenerateWeeklyReportPdf(InsightsOnStudentAttendance model)
        {
            var document = new Document();

            // Define Styles
            Style style = document.Styles["Normal"];
            style.Font.Name = "Segoe UI"; // Close to Nunito
            style.Font.Size = 10;

            Section section = document.AddSection();
            section.PageSetup.TopMargin = "1cm";
            section.PageSetup.LeftMargin = "1.5cm";
            section.PageSetup.RightMargin = "1.5cm";

            // --- HEADER ---
            var headerTable = section.AddTable();
            headerTable.AddColumn("18cm");
            var headerRow = headerTable.AddRow();
            headerRow.Height = "2cm";
            headerRow.VerticalAlignment = VerticalAlignment.Center;
            headerRow.Shading.Color = Color.FromRgb(232, 82, 26); // #E8521A

            var title = headerRow.Cells[0].AddParagraph("Principal's Analytics Dashboard");
            title.Format.Font.Size = 22;
            title.Format.Font.Bold = true;
            title.Format.Font.Color = Colors.White;
            title.Format.Alignment = ParagraphAlignment.Center;

            section.AddParagraph().Format.SpaceAfter = "0.5cm";

            // --- STATS GRID (Simulated using 3 columns) ---
            var statsTable = section.AddTable();
            statsTable.AddColumn("5.5cm");
            statsTable.AddColumn("0.5cm"); // Spacer
            statsTable.AddColumn("5.5cm");
            statsTable.AddColumn("0.5cm"); // Spacer
            statsTable.AddColumn("5.5cm");

            var statsRow = statsTable.AddRow();
            statsRow.Height = "3cm";

            // Box 1: Total Enrolled
            RenderStatBox(statsRow.Cells[0], "TOTAL ENROLLED", model.TotalStudents.ToString());
            // Box 2: Present Today
            RenderStatBox(statsRow.Cells[2], "PRESENT TODAY", model.PresentToday.ToString());
            // Box 3: Attendance Rate
            RenderStatBox(statsRow.Cells[4], "DAILY RATE", $"{model.DailyAttendanceRate}%");

            section.AddParagraph().Format.SpaceAfter = "1cm";

            // --- DANGER ZONE TABLE ---
            var dangerTitle = section.AddParagraph("⚠️ Chronic Absenteeism Warning");
            dangerTitle.Format.Font.Size = 16;
            dangerTitle.Format.Font.Bold = true;
            dangerTitle.Format.Font.Color = Color.FromRgb(211, 47, 47); // #d32f2f
            dangerTitle.Format.SpaceAfter = "0.2cm";

            var subtitle = section.AddParagraph("Students flagged for missing 3 or more days of school.");
            subtitle.Format.Font.Color = Colors.Gray;
            subtitle.Format.SpaceAfter = "0.5cm";

            if (model.AtRiskStudents.Count == 0)
            {
                var successBox = section.AddParagraph("🎉 Great news! No students are currently hitting the threshold.");
                successBox.Format.Alignment = ParagraphAlignment.Center;
                successBox.Format.Borders.Width = 0.5;
                successBox.Format.Borders.Color = Colors.Green;
                successBox.Format.Shading.Color = Colors.Honeydew;
                successBox.Format.SpaceBefore = "10pt";
                successBox.Format.SpaceAfter = "10pt";
                successBox.Format.LeftIndent = "10pt";
                successBox.Format.RightIndent = "10pt";
            }
            else
            {
                var table = section.AddTable();
                table.Borders.Width = 0.25;
                table.Borders.Color = Color.FromRgb(255, 205, 210);

                table.AddColumn("8cm");  // Name
                table.AddColumn("5cm");  // Absences
                table.AddColumn("5cm");  // Status

                var row = table.AddRow();
                row.Shading.Color = Color.FromRgb(255, 235, 238);
                row.Format.Font.Bold = true;
                row.Cells[0].AddParagraph("STUDENT NAME");
                row.Cells[1].AddParagraph("TOTAL ABSENCES");
                row.Cells[2].AddParagraph("STATUS");

                foreach (FlaggedStudent student in model.AtRiskStudents)
                {
                    row = table.AddRow();
                    row.VerticalAlignment = VerticalAlignment.Center;

                    // Student Name
                    row.Cells[0].AddParagraph(student.Name);

                    // Total Absences (using the Badge style)
                    var pAbsence = row.Cells[1].AddParagraph($"{student.TotalAbsences} Days");
                    pAbsence.Format.Font.Color = Colors.White;
                    row.Cells[1].Shading.Color = Color.FromRgb(211, 47, 47);

                    // Status Column
                    row.Cells[2].AddParagraph("Action Required");
                    row.Cells[2].Format.Font.Color = Color.FromRgb(211, 47, 47);
                    row.Cells[2].Format.Font.Bold = true;
                }
            }

            // Footer
            var footer = section.Footers.Primary.AddParagraph();
            footer.AddText($"Generated on {DateTime.Now:f} | Hybrid Learning Academy");
            footer.Format.Alignment = ParagraphAlignment.Center;
            footer.Format.Font.Size = 8;

            // Render to memory stream
            var renderer = new PdfDocumentRenderer { Document = document };
            renderer.RenderDocument();

            using var stream = new MemoryStream();
            renderer.PdfDocument.Save(stream);
            return stream.ToArray();
        }

        private void RenderStatBox(Cell cell, string label, string value)
        {
            cell.Shading.Color = Colors.White;
            cell.Format.Borders.Width = 1.5;
            cell.Format.Borders.Color = Color.FromRgb(245, 217, 184);

            var pLabel = cell.AddParagraph(label);
            pLabel.Format.Alignment = ParagraphAlignment.Center;
            pLabel.Format.Font.Size = 9;
            pLabel.Format.Font.Bold = true;
            pLabel.Format.Font.Color = Color.FromRgb(184, 92, 42);
            pLabel.Format.SpaceBefore = "0.5cm";

            var pValue = cell.AddParagraph(value);
            pValue.Format.Alignment = ParagraphAlignment.Center;
            pValue.Format.Font.Size = 24;
            pValue.Format.Font.Bold = true;
            pValue.Format.Font.Color = Color.FromRgb(232, 82, 26);
        }

        public async Task SendWeeklyEmail(string recipientEmail, byte[] pdfBytes)
        {
            using var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("Hybridlearnacademy05@gmail.com");
            mailMessage.To.Add(recipientEmail);
            mailMessage.Subject = $"Weekly Attendance Report - {DateTime.Now:dd MMM yyyy}";
            mailMessage.Body = "Please find attached the weekly Principal's Analytics Dashboard report.";

            using var stream = new MemoryStream(pdfBytes);
            mailMessage.Attachments.Add(new Attachment(stream, "AttendanceReport.pdf", "application/pdf"));

            using var client = new SmtpClient("Smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("Hybridlearnacademy05@gmail.com", "utxw xbwc dsik yysx");

            await client.SendMailAsync(mailMessage);
        }
    }
}