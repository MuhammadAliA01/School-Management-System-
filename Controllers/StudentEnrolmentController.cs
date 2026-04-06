using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;
using System.Net;
using System.Net.Mail;

namespace SchoolManagmentSystem.Controllers
{
    public class StudentEnrolmentController : Controller
    {

        private readonly ApplicationDBContext _context;

        public StudentEnrolmentController(ApplicationDBContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index(int studentID)
        {


            var parentId = User.FindFirst("UserInternalID")?.Value;

            
            ViewBag.studentID = studentID;

            var canAccess = HttpContext.Session.GetString("AccessnonReg");

            if (canAccess != "true")
            {
                return RedirectToAction("ParentDashboard", "Dashboards", new { parentID = parentId });
            }


            return View();
        }

        

        public IActionResult RegistrationService()
        {
            return View();
        }

        [Authorize]
        public IActionResult RegistrationConfirmation(int studentId)
        {
            if (TempData["FormSubmitted"] == null)
            {
                return RedirectToAction("HomePage", "Home");
            }


            var student = _context.Students
                                  .FirstOrDefault(s => s.StudentId == studentId);

            if (student == null)
                return Content("Student not found");

            return View(student); // This will use your RegistrationConfirmation.cshtml view
        }

        public IActionResult PreviewPdf(int studentId)
        {
            var parentId = User.FindFirst("UserInternalID")?.Value;


            var canAccess = HttpContext.Session.GetString("AccessnonReg");

            if (canAccess != "true")
            {
                return RedirectToAction("ParentDashboard", "Dashboards", new { parentID = parentId });
            }



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

            var parentId = _context.StudentRegistrations.Where(p => p.Id == studentId).Select(p => p.ParentId).FirstOrDefault();    
            var parentEmail = _context.Parents.Where(p=> p.Id == parentId).Select(p=> p.Email).FirstOrDefault();

            var parent = _context.Parents.Where(p => p.Id == parentId).Select(p => p).FirstOrDefault();


            var registrationService = new RegistrationService(_context);
            var document = registrationService.GetRegistration(studentId);



            using (MemoryStream stream = new MemoryStream())
            {

                document.Save(stream);
                byte[] bytes = stream.ToArray();

                var filePath = Path.Combine("wwwroot/PDFUploads", $"ProofOfRegistration_{studentId}.pdf");
                System.IO.File.WriteAllBytes(filePath, bytes);

                SmtpClient client = new SmtpClient("Smtp.gmail.com");
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("Hybridlearnacademy05@gmail.com", "utxw xbwc dsik yysx");
                MailAddress mailAddress = new MailAddress("Hybridlearnacademy05@gmail.com");
                MailAddress recipientAddress = new MailAddress(parentEmail);
                Attachment attachement = new Attachment(filePath);
                MailMessage mailMessage = new MailMessage();

                mailMessage.From = mailAddress;
                mailMessage.To.Add(recipientAddress);
                mailMessage.Subject = " Registration Successful!";
                mailMessage.Body = "Your registration has been successfully completed. Please find your Proof of Registration attached.";
                mailMessage.Attachments.Add(attachement);

                

                client.SendMailAsync(mailMessage);

            }

            if (student == null)
                return Content("Student not found");

            TempData["FormSubmitted"] = true;

            var studentMain = new Student
            {
                ParentId = student.ParentId,
                ParentName = parent.FirstName,
                ParentEmail = parent.Email,
                ParentPhone = "0",
                TeacherId = 3,
                Street = student.Street,
                Suburb = student.Suburb,
                City = student.City,
                PostalCode = student.PostalCode,
                FullName = student.ChildName,
                ChildDob = student.ChildDob,
                Gender = student.Gender,
                AutismLevel = student.AutismLevel,
                AutismType = student.AutismType,
                PdfPath = student.PdfPath,
                Comments = student.Comments,
                RegistrationDate = DateTime.Today,
                TotalFees = 60000,
                AmountPaid = 5000

            };

            _context.Students.Add(studentMain);

            _context.StudentRegistrations.Remove(student);

            _context.SaveChanges();

            return RedirectToAction("RegistrationConfirmation", new { studentId = studentMain.StudentId });
        }


        public IActionResult DownloadPdf(int studentId)
        {
            var parentId = User.FindFirst("UserInternalID")?.Value;


            var canAccess = HttpContext.Session.GetString("AccessnonReg");

            if (canAccess != "true")
            {
                return RedirectToAction("ParentDashboard", "Dashboards", new { parentID = parentId });
            }

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
