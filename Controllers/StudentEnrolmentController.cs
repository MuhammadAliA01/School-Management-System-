using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;

namespace SchoolManagmentSystem.Controllers
{
    public class StudentEnrolmentController : Controller
    {

        private readonly ApplicationDBContext _context;

        public StudentEnrolmentController(ApplicationDBContext context)
        {
            _context = context;
        }


        public IActionResult Index(int studentID)
        {
            ViewBag.studentID = studentID;


            return View();
        }

        public IActionResult RegistrationService()
        {
            return View();
        }

        public IActionResult RegistrationConfirmation(int studentId)
        {
            var student = _context.StudentRegistrations
                                  .FirstOrDefault(s => s.Id == studentId);

            if (student == null)
                return Content("Student not found");

            return View(student); // This will use your RegistrationConfirmation.cshtml view
        }

        public IActionResult PreviewPdf(int studentId)
        {
            var service = new RegistrationService(_context);
            var document = service.GetPreviewRegistration(studentId);

            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream);
                return File(stream.ToArray(), "application/pdf"); // preview
            }
        }

        public IActionResult Enrol(int studentId)
        {
            var student = _context.StudentRegistrations
                                  .FirstOrDefault(s => s.Id == studentId);

            if (student == null)
                return Content("Student not found");

            return RedirectToAction("RegistrationConfirmation", new { studentId = studentId });
        }


        public IActionResult DownloadPdf(int studentId)
        {

            try
            {
                var registrationService = new RegistrationService(_context);
                var document = registrationService.GetRegistration(studentId);

                using (MemoryStream stream = new MemoryStream())
                {
                    document.Save(stream);
                    byte[] bytes = stream.ToArray();

                    // This returns the file directly to the browser
                    return File(bytes, "application/pdf", "Proof of Registration.pdf");
                }
            }
            catch
            {
                // If student not found, return a small PDF saying so
                var doc = new MigraDoc.DocumentObjectModel.Document();
                var section = doc.AddSection();
                var paragraph = section.AddParagraph();
                paragraph.AddFormattedText("Student not found", MigraDoc.DocumentObjectModel.TextFormat.Bold);

                var pdfRenderer = new MigraDoc.Rendering.PdfDocumentRenderer();
                pdfRenderer.Document = doc;
                pdfRenderer.RenderDocument();

                using (MemoryStream stream = new MemoryStream())
                {
                    pdfRenderer.PdfDocument.Save(stream);
                    byte[] bytes = stream.ToArray();

                    return File(bytes, "application/pdf", "StudentNotFound.pdf");
                }
            }
        }
    }
}
