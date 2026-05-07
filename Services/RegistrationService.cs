using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Fields;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using SchoolManagmentSystem.Models;


namespace SchoolManagmentSystem.Services
{
    public class RegistrationService
    {
        private readonly ApplicationDBContext _context;

        public PdfDocument GetPreviewRegistration(int studentId)
        {
            var document = new Document();

            var student = _context.StudentRegistrations
                                  .FirstOrDefault(s => s.Id  == studentId);

            if (student == null)
                throw new Exception("Student not found");

            Section section = document.AddSection();

            
            //Adding an image
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/PDFImages/HbaLogo.jpeg");
            Image image = section.AddImage(imagePath);

            image.Width = "8cm";
            image.Height = "5cm";
            image.LockAspectRatio = true;
            image.Left = ShapePosition.Center;
            var spacer = section.AddParagraph();

            spacer.Format.SpaceBefore = "0.005cm";
            var spacer1 = section.AddParagraph();
            spacer1.Format.SpaceAfter = 9;
            //space before the image(top)
            section.PageSetup.TopMargin = "1cm";


            // Title
            var paragraph = section.AddParagraph();
            paragraph.AddFormattedText("REVIEW YOUR REGISTRATION DETAILS", TextFormat.Bold);
            paragraph.Format.Font.Size = 14;
            paragraph.Format.SpaceAfter = 10;
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            paragraph = section.AddParagraph();
            paragraph.AddText("Please review the information below carefully and ensure all details are correct before proceeding with registration.");
            paragraph.Format.SpaceAfter = 15;
            paragraph.Format.Alignment = ParagraphAlignment.Center;



            // PARENT TABLE
            var parentTable = section.AddTable();
            parentTable.Borders.Width = 0.25;
            parentTable.Format.Font.Size = 10;

            parentTable.AddColumn("5cm");
            parentTable.AddColumn("10cm");

            // Header row (grey bar)
            var row = parentTable.AddRow();
            row.Cells[0].MergeRight = 1;
            row.Cells[0].AddParagraph("Parent Information");
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.LightGray;

            // Data rows
            row = parentTable.AddRow();
            row.Cells[0].AddParagraph("Name:");
            row.Cells[1].AddParagraph(student.ParentName);

            row = parentTable.AddRow();
            row.Cells[0].AddParagraph("Email:");
            row.Cells[1].AddParagraph(student.ParentEmail);

            row = parentTable.AddRow();
            row.Cells[0].AddParagraph("Phone:");
            row.Cells[1].AddParagraph(student.ParentPhone);
            //line below centres the table
            parentTable.Rows.Alignment = RowAlignment.Center;

            //Add space between tables
            var spacer2 = section.AddParagraph();
            spacer2.Format.SpaceBefore = 10; // space above next table
            spacer2.Format.SpaceAfter = 10;  // space below previous table



            // STUDENT TABLE
            var studentTable = section.AddTable();
            studentTable.Borders.Width = 0.25;
            studentTable.Format.Font.Size = 10;

            studentTable.AddColumn("5cm");
            studentTable.AddColumn("10cm");

            // Header row
            row = studentTable.AddRow();
            row.Cells[0].MergeRight = 1;
            row.Cells[0].AddParagraph("Student Information");
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.LightGray;

            row = studentTable.AddRow();
            row.Cells[0].AddParagraph("Name:");
            row.Cells[1].AddParagraph(student.ChildName);

            row = studentTable.AddRow();
            row.Cells[0].AddParagraph("Date of Birth:");
            row.Cells[1].AddParagraph(student.ChildDob.ToString("dd MMMM yyyy"));

            row = studentTable.AddRow();
            row.Cells[0].AddParagraph("Gender:");
            row.Cells[1].AddParagraph(student.Gender);

            row = studentTable.AddRow();
            row.Cells[0].AddParagraph("Autism Level:");
            row.Cells[1].AddParagraph(student.AutismLevel);

            row = studentTable.AddRow();
            row.Cells[0].AddParagraph("Autism Type:");
            row.Cells[1].AddParagraph(student.AutismType);
            //line below centres the table
            studentTable.Rows.Alignment = RowAlignment.Center;

            //Add space between tables
            var spacer3 = section.AddParagraph();
            spacer3.Format.SpaceBefore = 10; // space above next table
            spacer3.Format.SpaceAfter = 10;  // space below previous table



            // ADDRESS TABLEE
            var addressTable = section.AddTable();
            addressTable.Borders.Width = 0.25;
            addressTable.Format.Font.Size = 10;

            addressTable.AddColumn("5cm");
            addressTable.AddColumn("10cm");

            // Header row
            row = addressTable.AddRow();
            row.Cells[0].MergeRight = 1;
            row.Cells[0].AddParagraph("Address");
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.LightGray;

            row = addressTable.AddRow();
            row.Cells[0].AddParagraph("Street:");
            row.Cells[1].AddParagraph(student.Street);

            row = addressTable.AddRow();
            row.Cells[0].AddParagraph("Suburb:");
            row.Cells[1].AddParagraph(student.Suburb);

            row = addressTable.AddRow();
            row.Cells[0].AddParagraph("City:");
            row.Cells[1].AddParagraph(student.City);

            row = addressTable.AddRow();
            row.Cells[0].AddParagraph("Postal Code:");
            row.Cells[1].AddParagraph(student.PostalCode);
            addressTable.Rows.Alignment = RowAlignment.Center;

            //Text at the bottom of pdf
            paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText("Hybrid Learning Academy");
            //line below centres the table
            paragraph.Format.Alignment = ParagraphAlignment.Center;



            var renderer = new PdfDocumentRenderer();
            renderer.Document = document;
            renderer.RenderDocument();

            return renderer.PdfDocument;

          
        }
        
        public RegistrationService(ApplicationDBContext context)
        {
            _context = context;
        }
        public PdfDocument GetRegistration(int studentId)
        {
            var document = new Document();
            

            // Fetch student from DB
            var student = _context.Students
                                  .FirstOrDefault(s => s.StudentId == studentId); 

            if (student == null)
                throw new Exception("Student not found");

            BuildDocument(document, student);

            var pdfRenderer = new PdfDocumentRenderer();
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            return pdfRenderer.PdfDocument;
        }

        
        private void BuildDocument(Document document, Student student)
        {
         

            //Each MigraDoc document needs at least one section
            Section section = document.AddSection();
            section.PageSetup.TopMargin = "1cm";
            


            //Adding an image
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/PDFImages/HbaLogo.jpeg");
            Image image = section.AddImage(imagePath);

            image.Width = "8cm";
            image.Height = "5cm";
            image.LockAspectRatio = true;
            image.Left = ShapePosition.Center;
            var spacer = section.AddParagraph();
           
            spacer.Format.SpaceBefore = "0.005cm";


            //Add sender Details
            var paragraph = section.AddParagraph();
            paragraph.AddFormattedText("HYBRID LEARNING ACADEMY" , TextFormat.Bold);
            paragraph.AddLineBreak();
            paragraph.Format.SpaceAfter = "1cm";
            paragraph.AddFormattedText("Principal: ", TextFormat.Bold); // only this part bold
            paragraph.AddText("Mrs Tasania Nadasan");
            paragraph.AddLineBreak();
            paragraph.AddFormattedText("Email: ", TextFormat.Bold); // only this part bold
            paragraph.AddText("joelene.nadasan@gmail.com");
            paragraph.AddLineBreak();
            paragraph.AddFormattedText("Phone: ", TextFormat.Bold); // only this part bold
            paragraph.AddText("065 362 9055");
            paragraph.Format.SpaceAfter = "0.5cm";
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            //Student details

            paragraph = section.AddParagraph();
            paragraph.AddFormattedText("PROOF OF REGISTRATION FOR:", TextFormat.Bold);
            paragraph.AddLineBreak();

            paragraph.AddFormattedText("Student Name: ", TextFormat.Bold);
            var studentName = paragraph.AddFormattedText(student.FullName);
            studentName.Bold = false;
            paragraph.AddLineBreak();

            paragraph.AddFormattedText("Date Of Birth: ", TextFormat.Bold);
            var dob = paragraph.AddFormattedText(student.ChildDob.ToString("yyyy-MM-dd"));
            dob.Bold = false;
            paragraph.AddLineBreak();

            paragraph.AddFormattedText("Phone: ", TextFormat.Bold);
            var phone = paragraph.AddFormattedText(student.ParentPhone);
            phone.Bold = false;

            paragraph.Format.SpaceAfter = 20;
            paragraph.Format.Alignment = ParagraphAlignment.Center;


            // Layout for date
            var infoTable = section.AddTable();
            infoTable.Borders.Width = 0;

            infoTable.AddColumn("7.06cm");
            infoTable.AddColumn("7.06cm");

            var rowInfo = infoTable.AddRow();
            infoTable.Rows.Alignment = RowAlignment.Center;

            // Date&Time on the right
            var leftPara = rowInfo.Cells[1].AddParagraph();
            leftPara.Add(new DateField { Format = "dd MMMM yyyy, HH:mm" });
            leftPara.Format.Alignment = ParagraphAlignment.Right;

            // Space after
            var spacer2 = section.AddParagraph();
            spacer2.Format.SpaceAfter = "0.2cm";



            string[,] subjects =
            {
                {"1" , "Creative Development" , "Performance Arts" , "A" , "R" + "1800" },
                {"2" , "Oral skills and Language Development" , "Understanding language and expressing language"  , "A" , "R" + "1200" },
                {"3" , "Personal Independance" , "Toilet Training, Hand washing , Dental Care , Dressing Up" , "A" , "R" + "1000"},
                {"4" , "Basic Concepts", "Language Concepts, Identify, match and sequence" , "A" , "R" + "1500"},
                {"5" , "Physical and Social Development" , " Gross and Fine motor Skills" , "A" , "R" + "2000"},
                {"6" , "Social Skills" , "Self recognition, sharing, group work, becoming more independant"  , "A" ,"R" + "1900"},

            };



            //create subject table

            var table = document.LastSection.AddTable();
            table.Borders.Width = 0.5;
            table.Rows.Alignment = RowAlignment.Center;

            //define colums first then rows

            table.AddColumn("1cm");
            table.AddColumn("4cm");
            table.AddColumn("5cm");
            table.AddColumn("2cm");
            table.AddColumn("2cm");


            //Create the header of the table
            
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Font.Bold = true;
            paragraph.Format.Font.Bold = true;

            row.Shading.Color = MigraDoc.DocumentObjectModel.Color.FromRgb(211, 211, 211); // light grey shading for the column heading

            row.Cells[0].AddParagraph("No");
            row.Cells[1].AddParagraph("Subject");
            row.Cells[2].AddParagraph("Description");
            row.Cells[3].AddParagraph("Class Group");
            row.Cells[4].AddParagraph("Amount (Yearly)");

          
            for (int i = 0; i < subjects.GetLength(0); i++)
            {
                row = table.AddRow();
                row.Cells[0].AddParagraph(subjects[i, 0]);
                row.Cells[1].AddParagraph(subjects[i, 1]);
                row.Cells[2].AddParagraph(subjects[i, 2]);
                row.Cells[3].AddParagraph(subjects[i, 3]);
                row.Cells[4].AddParagraph(subjects[i, 4]);
              

            }
            

            // CalculateS total balance 
            decimal total = 0;

            for (int i = 0; i < subjects.GetLength(0); i++)
            {
                string amtString = subjects[i, 4].Replace("R", ""); 
                decimal amt = decimal.Parse(amtString);
                total += amt;
            }

            // Set your Balance paragraph text 
            paragraph = section.AddParagraph();
            paragraph.AddText("Outstanding Balance: R" + total.ToString("N2"));
            paragraph.AddLineBreak();
            paragraph.Format.SpaceBefore = "1cm";
            paragraph.Format.SpaceAfter = "0.3cm";
            paragraph.Format.LeftIndent = "1cm";


            //Adding the signature image
            var imagePath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/PDFImages/Signature.jpeg");
            Image image2 = section.AddImage(imagePath2);

            image2.Width = "5cm";
            image2.Height = "3cm";
            image2.LockAspectRatio = true;
            image2.Left = "1cm";
            var spacer1 = section.AddParagraph();


            //Name under the signature

            paragraph = section.AddParagraph();
            paragraph.AddText("Principal: Mrs Tasania Joelene Nadasan");
            paragraph.AddLineBreak();
            paragraph.Format.SpaceBefore = "0.05cm";
            paragraph.Format.LeftIndent = "1cm";

            //Bottom of page
            paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText("Hybrid Learning Academy");
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            
        }
    }
}

