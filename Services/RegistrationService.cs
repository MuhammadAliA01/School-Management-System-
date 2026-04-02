using Microsoft.VisualBasic;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Fields;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;

namespace SchoolManagmentSystem.Services
{
    public class RegistrationService
    {
        public PdfDocument GetRegistration()
        {
            //creating a MigraDoc document
            var document = new Document();

            BuildDocument(document);

           //creating a renderer for the MigraDoc document
           var pdfRenderer = new PdfDocumentRenderer();
           pdfRenderer.Document = document;

            //layout and render document to pdf
            pdfRenderer.RenderDocument();

            return pdfRenderer.PdfDocument;
        }

       
        private void BuildDocument(Document document)
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
            paragraph.AddFormattedText("Principal: ", TextFormat.Bold); // only this part bold
            paragraph.AddText("Ms Tasania Nadasan");
            paragraph.AddLineBreak();
            paragraph.AddFormattedText("Email: ", TextFormat.Bold); // only this part bold
            paragraph.AddText("joelene.nadasan@gmail.com");
            paragraph.AddLineBreak();
            paragraph.AddFormattedText("Phone: ", TextFormat.Bold); // only this part bold
            paragraph.AddText("065 362 9055");
            paragraph.Format.SpaceAfter = 20;

            //Student details

            paragraph = section.AddParagraph();
            paragraph.AddFormattedText("PROOF OF REGISTRATION FOR:" , TextFormat.Bold);
            paragraph.AddLineBreak();
            paragraph.AddText("Name:");
            paragraph.AddLineBreak();
            paragraph.AddText("Surname:");
            paragraph.AddLineBreak();
            paragraph.AddText("Birth Certificate Number:");
            paragraph.AddLineBreak();
            paragraph.AddText("Date Of Birth:");
            paragraph.AddLineBreak();
            paragraph.AddText("Medical Records:");
            paragraph.AddLineBreak();
            paragraph.AddText("Phone:");
            paragraph.Format.SpaceAfter = 20;


            paragraph = section.AddParagraph();
            paragraph.AddText("Student Number: 224034567");
            paragraph.AddLineBreak();
            paragraph.Add(new DateField { Format = "yyyy/MM/dd : hh:mm " });
            paragraph.Format.SpaceAfter = 10;


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
            paragraph.AddText("Balance: R" + total.ToString("N2"));
            paragraph.AddLineBreak();
            paragraph.Format.SpaceBefore = "1cm";
            paragraph.Format.SpaceAfter = "0.3cm";


            //Adding the signature image
            var imagePath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/PDFImages/Signature.jpeg");
            Image image2 = section.AddImage(imagePath2);

            image2.Width = "5cm";
            image2.Height = "3cm";
            image2.LockAspectRatio = true;
            image2.Left = ShapePosition.Left;
            var spacer1 = section.AddParagraph();


            //Name under the signature

            paragraph = section.AddParagraph();
            paragraph.AddText("Principal: Mrs Tasania Joelene Nadasan");
            paragraph.AddLineBreak();
            paragraph.Format.SpaceBefore = "0.05cm";

            //Bottom of page
            paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText("Hybrid Learning Academy");
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            
        }
    }
}

