using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using SchoolManagmentSystem.Models;
using System.IO;

namespace SchoolManagmentSystem.Services
{
    public class EmploymentService
    {
        private readonly ApplicationDBContext _context;

        public EmploymentService(ApplicationDBContext context)
        {
            _context = context;
        }

        public PdfDocument GetEmploymentContract(int applicationId)
        {
            var document = new Document();

            // 1. Fetch the application details from the database
            var application = _context.EmploymentApplications
                                      .FirstOrDefault(a => a.id == applicationId);

            if (application == null)
                throw new Exception("Application not found");

            // 2. Build the visual structure
            BuildContractDocument(document, application);

            // 3. Render to PDF
            var renderer = new PdfDocumentRenderer();
            renderer.Document = document;
            renderer.RenderDocument();

            return renderer.PdfDocument;
        }

        private void BuildContractDocument(Document document, EmploymentApplication app)
        {
            Section section = document.AddSection();
            section.PageSetup.TopMargin = "1.5cm";
            section.PageSetup.BottomMargin = "1.5cm";

            // --- LOGO ---
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/PDFImages/HbaLogo.jpeg");
            if (File.Exists(imagePath))
            {
                Image image = section.AddImage(imagePath);
                image.Width = "6cm";
                image.LockAspectRatio = true;
                image.Left = ShapePosition.Center;
            }

            // --- TITLE ---
            var title = section.AddParagraph();
            title.Format.SpaceBefore = "1cm";
            title.Format.Alignment = ParagraphAlignment.Center;
            title.AddFormattedText("OFFICIAL CONTRACT OF EMPLOYMENT", TextFormat.Bold);
            title.Format.Font.Size = 16;

            // --- EMPLOYEE DETAILS TABLE ---
            var table = section.AddTable();
            table.Borders.Width = 0.25;
            table.Format.SpaceBefore = "1cm";
            table.AddColumn("5cm");
            table.AddColumn("11cm");

            var row = table.AddRow();
            row.Shading.Color = Colors.LightGray;
            row.Cells[0].MergeRight = 1;
            row.Cells[0].AddParagraph("Employee Personal Information").Format.Font.Bold = true;

            row = table.AddRow();
            row.Cells[0].AddParagraph("Full Name:");
            row.Cells[1].AddParagraph($"{app.FirstName} {app.LastName}");

            row = table.AddRow();
            row.Cells[0].AddParagraph("Position:");
            row.Cells[1].AddParagraph(app.selectedRole);

            row = table.AddRow();
            row.Cells[0].AddParagraph("Reference Number:");
            row.Cells[1].AddParagraph(app.referenceNumber ?? "N/A");

            table.Rows.Alignment = RowAlignment.Center;

            // --- CONTRACT TERMS ---
            var terms = section.AddParagraph();
            terms.Format.SpaceBefore = "1cm";
            terms.AddFormattedText("1. APPOINTMENT", TextFormat.Bold);
            terms.AddLineBreak();
            terms.AddText($"The Employer hereby appoints the Employee in the capacity of {app.selectedRole}.");

            terms.AddLineBreak();
            terms.AddLineBreak();
            terms.AddFormattedText("2. JOB DESCRIPTION", TextFormat.Bold);
            terms.AddLineBreak();

            // Dynamic Job Description based on Role
            if (app.selectedRole.ToLower().Contains("teacher"))
            {
                terms.AddText("The Teacher is responsible for lesson planning, student assessment, and maintaining classroom discipline for children aged 1-10 within a special needs environment.");
            }
            else if (app.selectedRole.ToLower().Contains("cleaner"))
            {
                terms.AddText("The Cleaner is responsible for the sanitation and hygiene of school premises, ensuring a safe and clean environment for students and staff.");
            }
            else
            {
                terms.AddText("Duties as assigned by the School Principal in line with the school's operational requirements.");
            }

            terms.AddLineBreak();
            terms.AddLineBreak();
            terms.AddFormattedText("3. CONFIDENTIALITY", TextFormat.Bold);
            terms.AddLineBreak();
            terms.AddText("The Employee agrees to maintain strict confidentiality regarding student medical records and behavioral data at all times.");

            // --- SIGNATURE SECTION ---
            var signaturePara = section.AddParagraph();
            signaturePara.Format.SpaceBefore = "2cm";

            // Principal Signature Image
            var sigPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/PDFImages/Signature.jpeg");
            if (File.Exists(sigPath))
            {
                Image sigImage = section.AddImage(sigPath);
                sigImage.Width = "4cm";
                sigImage.Left = "1cm";
            }

            var signTable = section.AddTable();
            signTable.Format.SpaceBefore = "0.5cm";
            signTable.AddColumn("8cm");
            signTable.AddColumn("8cm");

            var signRow = signTable.AddRow();
            signRow.Cells[0].AddParagraph("__________________________\nPrincipal Signature");
            signRow.Cells[1].AddParagraph("__________________________\nEmployee Signature");

            // --- FOOTER ---
            var footer = section.Footers.Primary.AddParagraph();
            footer.AddText("Hybrid Learning Academy - Confidential Employment Document");
            footer.Format.Alignment = ParagraphAlignment.Center;
            footer.Format.Font.Size = 9;
            footer.Format.Font.Italic = true;
        }
    }
}