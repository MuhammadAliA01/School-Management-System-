using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mscc.GenerativeAI.Types;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;
using SchoolManagmentSystem.Controllers;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Claims;

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

        [Authorize]
        public IActionResult RegForm()
        {

            var parentId = User.FindFirst("UserInternalID")?.Value;

            var access = HttpContext.Session.GetString("AccessnoStudent");

            if (access != "true")
            {
                return RedirectToAction("ParentDashboard", "Dashboards", new {parentID = parentId });
            }



            return View();
        }



        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Confirmation(StudentRegistration model, List<IFormFile> pdfFiles)
        {

            var userIdClaim = User.FindFirst("UserInternalID")?.Value;
            if (int.TryParse(userIdClaim, out int parentId))
            {
                model.ParentId = parentId;
            }


            model.ParentName = string.IsNullOrWhiteSpace(model.ParentName) ? "Not Provided" : model.ParentName;
            model.ParentEmail = string.IsNullOrWhiteSpace(model.ParentEmail) ? "notprovided@example.com" : model.ParentEmail;
            model.ParentPhone = string.IsNullOrWhiteSpace(model.ParentPhone) ? "000-000-0000" : model.ParentPhone;

            model.Street = string.IsNullOrWhiteSpace(model.Street) ? "N/A" : model.Street;
            model.Suburb = string.IsNullOrWhiteSpace(model.Suburb) ? "N/A" : model.Suburb;
            model.City = string.IsNullOrWhiteSpace(model.City) ? "N/A" : model.City;
            model.PostalCode = string.IsNullOrWhiteSpace(model.PostalCode) ? "0000" : model.PostalCode;

            model.ChildName = string.IsNullOrWhiteSpace(model.ChildName) ? "Unknown Student" : model.ChildName;
            model.Gender = string.IsNullOrWhiteSpace(model.Gender) ? "Not Specified" : model.Gender;
            model.AutismLevel = string.IsNullOrWhiteSpace(model.AutismLevel) ? "Pending Assessment" : model.AutismLevel;
            model.AutismType = string.IsNullOrWhiteSpace(model.AutismType) ? "Pending Assessment" : model.AutismType;
            model.Comments = string.IsNullOrWhiteSpace(model.Comments) ? "No comments" : model.Comments;


            if (model.ChildDob == default)
            {
                model.ChildDob = new DateTime(2000, 1, 1);
            }


            
            if (pdfFiles != null && pdfFiles.Count > 0) //ensures the code doesn't crash if the user clicks "Submit" without selecting any files. It only runs if there is actually work to do.
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");   //It finds the physical path to your wwwroot/uploads folder.
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                foreach (var file in pdfFiles)    //Since the user can upload multiple files, the code loops through them one by one. If they upload 3 PDFs, this loop runs 3 times.
                {
                    if (file.Length > 0)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;  //If two users upload a file named ID.pdf, the second one would overwrite the first. A GUID (a long string of random letters and numbers like 74a2...) makes the filename unique so no files are ever lost or replaced.
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))   //This takes the data sitting in your computer's "temporary memory" (RAM) and writes it onto the permanent hard drive in the uploads folder.
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        // Create a new document record for each file
                        model.Documents.Add(new StudentDocument
                        {
                            FilePath = "/uploads/" + uniqueFileName,         //It doesn't save the file in the database. Instead, it saves filePath and fileName
                            FileName = file.FileName
                        });
                    }
                }
            }


            model.RegistrationDate = DateTime.Now;
            model.Status = "Pending";
            model.feesPaid = false;


            try
            {

                ModelState.Clear();

                _context.StudentRegistrations.Add(model);
                await _context.SaveChangesAsync();

                return View("Confirmation", model);
            }
            catch (Exception ex)
            {

                _logger.LogError($"Database Save Error: {ex.Message}");
                return Content($"Error saving to database: {ex.InnerException?.Message ?? ex.Message}");
            }
        }





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




