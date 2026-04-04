using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using System.Diagnostics;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;
using SchoolManagmentSystem.Services;
using System.IO;
using System.Drawing;
using SchoolManagmentSystem.Controllers;

namespace SchoolManagmentSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDBContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDBContext context)
        {
            _logger = logger;
            _context = context;

        }

        public IActionResult HomePage()
        {

            
            return View();

            
        }

        public IActionResult FAQ()
        {
            var faqs = new List<FAQ_Model> {
            new FAQ_Model { Questions = "How can I create an account", Answers = "1.First navigate to the Create account page______2.Next fill in  Your Id Number,Name,Surname, and Email Address______3.Next create A Username and Password  (Password must cotain aleast 1 capital letter and have a length of 8 characters)"
            },
            new FAQ_Model { Questions = "What is the goals of our website", Answers = "Our website aims to provide a safe and secure service to help apply and register your child for our school without having you to apply physically or provide inconveniences for you and you child." },
            new FAQ_Model{ Questions = "What is the various grade offered by our school?", Answers = "Grades 1-8" },
            new FAQ_Model{ Questions = "How does the school support students with special needs?", Answers = "We offer comprehensive special education services, including personalized learning plans, resource rooms,interactive learning, and specialized and experienced couselors/therapist to monitor your child." },
            new FAQ_Model{ Questions = "Is there homework, and how much ?", Answers = "Yes, homework is assigned daily to reinforce learning. The amount depends on the child health and developementrecords that will be analyzed. In most cases it is 4-6 hours a week\r\n" },
            new FAQ_Model{ Questions = "What extracurricular activities are available?", Answers = "We offer a variety of sports, arts, music programs etc to aid in the developement of a child" }
        };
            return View(faqs);
        }




        public IActionResult ContactUs()
        {
            return View();
        }

        
        public IActionResult Gallery()
        {
            return View();
        }

        public IActionResult Calendar()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

<<<<<<< HEAD
        public IActionResult Index()
=======
        public IActionResult RegForm()
>>>>>>> f5f0ad85fc3e3974f0863847e25bec9aed6efad8
        {
            return View();
        }

<<<<<<< HEAD
     
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

            return View(student); // This will use your Confirmation.cshtml view
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
                                  .FirstOrDefault(s => s.Id == 2);

            if (student == null)
                return Content("Student not found");

            return RedirectToAction("Confirmation", new { studentId = studentId });
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
=======

        // Add the DB Context to your constructor
        


        // 1. The POST method that receives the form data
        [HttpPost]
        public async Task<IActionResult> RegForm(StudentRegistration registration, IFormFile pdfFile)
        {
            if (ModelState.IsValid)
            {
                // Set the registration date
                registration.RegistrationDate = DateTime.Now;

                // (Optional) Handle File Upload logic here if you are saving the PDF

                _context.StudentRegistrations.Add(registration);
                await _context.SaveChangesAsync();

                // Redirect to the Confirmation page, passing the new ID
                return RedirectToAction("Confirmation", "Home", new { id = registration.Id });
            }


            return View(registration);
        }

        // 2. The GET method that displays the confirmation
        public async Task<IActionResult> Confirmation(int id)
        {
            try
            {
                var registration = await _context.StudentRegistrations
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (registration == null) return NotFound();

                return View(registration);
            }
            catch (SqlException ex)
            {
                // This will redirect to your error page instead of showing the "Ugly" crash screen
                return RedirectToAction("Error");
            }
        }



>>>>>>> f5f0ad85fc3e3974f0863847e25bec9aed6efad8




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
public class MyFontResolver : IFontResolver
{
    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
        {
            if (isBold && isItalic) return new FontResolverInfo("arialbi.ttf");
            if (isBold) return new FontResolverInfo("arialbd.ttf");
            if (isItalic) return new FontResolverInfo("ariali.ttf");
            return new FontResolverInfo("arial.ttf");
        }

        if (familyName.Equals("Courier New", StringComparison.OrdinalIgnoreCase))
        {
            if (isBold && isItalic) return new FontResolverInfo("courbi.ttf");
            if (isBold) return new FontResolverInfo("courbd.ttf");
            if (isItalic) return new FontResolverInfo("couri.ttf");
            return new FontResolverInfo("cour.ttf");
        }

        if (familyName.Equals("Times New Roman", StringComparison.OrdinalIgnoreCase))
        {
            if (isBold && isItalic) return new FontResolverInfo("timesbi.ttf");
            if (isBold) return new FontResolverInfo("timesbd.ttf");
            if (isItalic) return new FontResolverInfo("timesi.ttf");
            return new FontResolverInfo("times.ttf");
        }

        return new FontResolverInfo("arial.ttf");
    }

    public byte[] GetFont(string faceName)
    {
        string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), faceName);

        if (File.Exists(fontPath))
        {
            return File.ReadAllBytes(fontPath);
        }

        return File.ReadAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf"));
    }
}




