using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.ServicesAi.EmploymentAI;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SchoolManagmentSystem.Controllers
{
    public class EmploymentController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _environment;

        public EmploymentController(ApplicationDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


      


        public IActionResult EmploymentInfo()
        {
            return View();
        }

        public async Task<IActionResult> ApplicationList()
        {
            return View(await _context.EmploymentApplications.ToListAsync());

        }

        public IActionResult SearchStatus(string searchString)
        {
            var applicationstatus = _context.EmploymentApplications.Where(a => a.referenceNumber == searchString)
               .Select(a => a.Status).FirstOrDefault();

            ViewBag.Status = applicationstatus;


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(StudentRegistration model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model); // Returns to form with error messages
        }



        public IActionResult SubmitApplication()
        {
            return View();
        }

        // POST: EmploymentApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitApplication([Bind("FirstName,LastName,Email,selectedRole")] EmploymentApplication employmentApplication, IFormFile cvFile)
        {



            var refNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            if (cvFile != null && cvFile.Length > 0)
            {
               





                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                var allowedfileTypes = new[] { ".pdf" };
                var extension = Path.GetExtension(cvFile.FileName).ToLowerInvariant();

                if (!allowedfileTypes.Contains(extension))
                {
                    ModelState.AddModelError("CVLocation", "Only PDF Files are accepted");
                }

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(cvFile.FileName);

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await cvFile.CopyToAsync(fileStream);
                }

                employmentApplication.CVLocation = "/uploads/" + uniqueFileName;


            }
            else
            {
                ModelState.AddModelError("", "Please upload your CV.");
                return View(employmentApplication);
            }

            bool emailExists = await _context.EmploymentApplications.AnyAsync(e => e.Email == employmentApplication.Email);

            if (emailExists)
            {
                ModelState.AddModelError("", "An application with this email already exists.");
                return View(employmentApplication);
            }

            
            if (ModelState.IsValid)
            {
                if (Regex.IsMatch(employmentApplication.Email, "(?=.*[@])"))
                {
                    SmtpClient client = new SmtpClient("Smtp.gmail.com");
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("Hybridlearnacademy05@gmail.com", "utxw xbwc dsik yysx");
                    MailAddress mailAddress = new MailAddress("Hybridlearnacademy05@gmail.com");
                    MailAddress recipientAddress = new MailAddress(employmentApplication.Email);

                    MailMessage mailMessage = new MailMessage();
                    var cvService = new CVService();
                    var aiService = new AIScoringService();
                    var cvText = await cvService.ExtractTextFromPDFAsync(cvFile);
                    var aiFeedback = await aiService.GetSuitabilityScore(cvText, employmentApplication.selectedRole);


                    employmentApplication.Status = "Pending";
                    employmentApplication.referenceNumber = refNumber;
                    employmentApplication.AIReviewSummary = aiFeedback.Summary;
                    employmentApplication.SuitabilityScore = aiFeedback.Score;

                    mailMessage.From = mailAddress;
                    mailMessage.To.Add(recipientAddress);
                    mailMessage.Subject = "HLA Job Application!";
                    mailMessage.Body = "Thank you for applying for a career at Hybrid Learning Academy! Your application reference number is " + employmentApplication.referenceNumber.ToString();

                    _context.Add(employmentApplication);
                    await _context.SaveChangesAsync();
                    await client.SendMailAsync(mailMessage);
                    return RedirectToAction("ApplicationConfirmed", "Employment", new { referenceNum = employmentApplication.referenceNumber });
                }
                else
                {
                    ModelState.AddModelError("", "Email is invalid");
                }



                
            }
            
            return View(employmentApplication);
        }


        public IActionResult ApplicationConfirmed(string referenceNum)
        {
            ViewBag.referenceNum = referenceNum;


            return View();
        }

    }
}
