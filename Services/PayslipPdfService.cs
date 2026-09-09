using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

    namespace SchoolManagmentSystem.Services
    {
        public class PayslipPdfService
        {
            public byte[] GeneratePdf()
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                 
                    });
                });

                return document.GeneratePdf();
            }
        }
    }
