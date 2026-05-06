using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;
using SchoolManagmentSystem.ServicesAi.EmploymentAI;
using Stripe;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SchoolManagmentSystem.Controllers
{
    public class EmploymentController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly AIScoringService _aiService;
        private readonly EmploymentService _employmentService;

        public EmploymentController(ApplicationDBContext context, IWebHostEnvironment environment, AIScoringService aiService, EmploymentService employmentService)
        {
            _context = context;
            _aiService = aiService;
            _environment = environment;
            _employmentService = employmentService;
        }


      


        public IActionResult EmploymentInfo()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> ApplicationList()
        {
            var parentId = User.FindFirst("UserInternalID")?.Value;

            if (User.IsInRole("Parent"))
            {
                return RedirectToAction("ParentDashboard", "Dashboards", new { parentID = parentId });
            }
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
                    var cvText = await cvService.ExtractTextFromPDFAsync(cvFile);
                    var aiFeedback = await _aiService.GetSuitabilityScore(cvText, employmentApplication.selectedRole, "", "");


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

        [Authorize]
        public async Task<IActionResult> ApplicationAccept(int id)
        {
            var application = _context.EmploymentApplications.Where(e => e.id == id).Select(e => e).SingleOrDefault();
            var email = application.Email;
            application.dateSent = DateTime.Now;
            var quizlink = "";
            if (application.selectedRole == "Teacher")
            {
                quizlink = Url.Action("TeacherQuiz", "Employment", new { id = application.id }, Request.Scheme);

            }
            else if (application.selectedRole == "Cleaner")
            {
                quizlink = Url.Action("CleanerQuiz", "Employment", new { id = application.id }, Request.Scheme);

            }


            SmtpClient client = new SmtpClient("Smtp.gmail.com");
            client.Port = 587;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("Hybridlearnacademy05@gmail.com", "utxw xbwc dsik yysx");
            MailAddress mailAddress = new MailAddress("Hybridlearnacademy05@gmail.com");
            MailAddress recipientAddress = new MailAddress(email);
            MailMessage mailMessage = new MailMessage();

            application.Status = "Quiz Sent";

            mailMessage.From = mailAddress;
            mailMessage.To.Add(recipientAddress);
            mailMessage.Subject = "HLA Job Application!";
            mailMessage.Body = $"Good Day! \n\n Your application has been accepted for further reviewing. \n Please complete the quiz linked below: \n {quizlink}";
            await _context.SaveChangesAsync();
            await client.SendMailAsync(mailMessage);


            return RedirectToAction("ApplicationList", "Employment");
        }

        [Authorize]
        public async Task<IActionResult> ApplicationReject(int id)
        {
            var application = _context.EmploymentApplications.Where(e => e.id == id).Select(e => e).SingleOrDefault();

            _context.Remove(application);
            await _context.SaveChangesAsync();


            return RedirectToAction("ApplicationList", "Employment");
        }

        public async Task<IActionResult> TeacherQuiz(int id)
        {
            var application = _context.EmploymentApplications.Where(e => e.id == id).Select(e => e).SingleOrDefault();

            if (application.dateSent.HasValue)
            {
                var expiryDate = application.dateSent.Value.AddDays(7);

                if (DateTime.Now > expiryDate)
                {
                    application.Status = "No Response";
                    await _context.SaveChangesAsync();
                    return RedirectToAction("LinkExpired", "Employment");
                }
            }

            return View(application);
        }

        public async Task<IActionResult> LinkExpired()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TeacherQuiz(EmploymentApplication application)
        {
            if (application.MCQ1 == null ||
                application.MCQ2 == null ||
                application.MCQ3 == null ||
                application.MCQ4 == null ||
                application.OEQ1 == null ||
                application.OEQ2 == null ||
                application.OEQ3 == null)
            {
                ModelState.AddModelError("", "Please fill in everything!");
                return View(application);
            }
            var DBRecord = _context.EmploymentApplications.Where(e => e.id == application.id).Select(e => e).SingleOrDefault();
            var cvFile = Path.GetFileName(DBRecord.CVLocation);
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", cvFile);
            var cvService = new CVService();
            var cvText = await cvService.ExtractTextFromPDFAsync(filePath);


            DBRecord.MCQ1 = application.MCQ1;
            DBRecord.MCQ2 = application.MCQ2;
            DBRecord.MCQ3 = application.MCQ3;
            DBRecord.MCQ4 = application.MCQ4;

            DBRecord.OEQ1 = application.OEQ1;
            DBRecord.OEQ2 = application.OEQ2;
            DBRecord.OEQ3 = application.OEQ3;
            DBRecord.Status = "Quiz Complete";

            string quizAnswers = "Q1 " + DBRecord.MCQ1 + "\n" +"Q2 " + DBRecord.MCQ2 + "\n" +"Q3 " + DBRecord.MCQ3 + "\n" +"Q4 " + DBRecord.MCQ4 + "\n" +"Q5 " + DBRecord.OEQ1 + "\n" +"Q6 " + DBRecord.OEQ2 + "\n" +"Q7 " + DBRecord.OEQ3;

            var aiFeedback = await _aiService.GetSuitabilityScore(cvText, DBRecord.selectedRole, quizAnswers, "");

            DBRecord.AIReviewSummary = aiFeedback.Summary;
            DBRecord.SuitabilityScore = aiFeedback.Score;


            await _context.SaveChangesAsync();


            return RedirectToAction("HomePage", "Home");
        }

        public async Task<IActionResult> CleanerQuiz(int id)
        {
            var application = _context.EmploymentApplications.Where(e => e.id == id).Select(e => e).SingleOrDefault();

            if (application.dateSent.HasValue)
            {
                var expiryDate = application.dateSent.Value.AddDays(7);

                if (DateTime.Now > expiryDate)
                {
                    application.Status = "No Response";
                    await _context.SaveChangesAsync();
                    return RedirectToAction("LinkExpired", "Employment");
                }
            }

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> CleanerQuiz(EmploymentApplication application)
        {
            if (application.MCQ1 == null ||
                application.MCQ2 == null ||
                application.MCQ3 == null ||
                application.MCQ4 == null ||
                application.OEQ1 == null ||
                application.OEQ2 == null ||
                application.OEQ3 == null)
            {
                ModelState.AddModelError("", "Please fill in everything!");
                return View(application);
            }
            var DBRecord = _context.EmploymentApplications.Where(e => e.id == application.id).Select(e => e).SingleOrDefault();
            var cvFile = Path.GetFileName(DBRecord.CVLocation);
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", cvFile);
            var cvService = new CVService();
            var cvText = await cvService.ExtractTextFromPDFAsync(filePath);


            DBRecord.MCQ1 = application.MCQ1;
            DBRecord.MCQ2 = application.MCQ2;
            DBRecord.MCQ3 = application.MCQ3;
            DBRecord.MCQ4 = application.MCQ4;

            DBRecord.OEQ1 = application.OEQ1;
            DBRecord.OEQ2 = application.OEQ2;
            DBRecord.OEQ3 = application.OEQ3;
            DBRecord.Status = "Quiz Complete";

            string quizAnswers = "Q1 " + DBRecord.MCQ1 + "\n" + "Q2 " + DBRecord.MCQ2 + "\n" + "Q3 " + DBRecord.MCQ3 + "\n" + "Q4 " + DBRecord.MCQ4 + "\n" + "Q5 " + DBRecord.OEQ1 + "\n" + "Q6 " + DBRecord.OEQ2 + "\n" + "Q7 " + DBRecord.OEQ3;

            var aiFeedback = await _aiService.GetSuitabilityScore(cvText, DBRecord.selectedRole, quizAnswers, "");

            DBRecord.AIReviewSummary = aiFeedback.Summary;
            DBRecord.SuitabilityScore = aiFeedback.Score;


            await _context.SaveChangesAsync();


            return RedirectToAction("HomePage", "Home");
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ScheduleInterview(int id, DateTime interviewDateTime, string interviewType, string locationDetails)
        {
            var application = _context.EmploymentApplications.Where(e => e.id == id).Select(e => e).SingleOrDefault();

            if (application != null)
            {
                application.Status = "Interview Scheduled";

                application.interviewDate = interviewDateTime;
                application.interviewType = interviewType;

                application.meetingLink = locationDetails;

                SmtpClient client = new SmtpClient("Smtp.gmail.com");
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("Hybridlearnacademy05@gmail.com", "utxw xbwc dsik yysx");
                MailAddress mailAddress = new MailAddress("Hybridlearnacademy05@gmail.com");
                MailAddress recipientAddress = new MailAddress(application.Email);
                MailMessage mailMessage = new MailMessage();

                mailMessage.From = mailAddress;
                mailMessage.To.Add(recipientAddress);
                mailMessage.Subject = "HLA Job Application!";

                if (application.interviewType == "In-Person")
                {
                    mailMessage.Body = $"Good day \n\n Your application has been reviewed and we would like to conduct an interview with you. \n\n Interview date is: {interviewDateTime.Date.ToShortDateString()} at {interviewDateTime.TimeOfDay} \n\n Location is {locationDetails}";
                }
                else if (application.interviewType == "Teams")
                {
                    mailMessage.Body = $"Good day \n\n Your application has been reviewed and we would like to conduct an interview with you. \n\n Interview date is: {interviewDateTime.Date} at {interviewDateTime.TimeOfDay} \n\n MS Teams Link: {locationDetails}";
                }



                    await _context.SaveChangesAsync();
                await client.SendMailAsync(mailMessage);
            }

            

            

            return RedirectToAction("ApplicationList");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitInterviewNotes(int id, string interviewNotes)
        {
            var application = _context.EmploymentApplications.Where(e => e.id == id).Select(e => e).SingleOrDefault();
            application.notes = interviewNotes;

            var cvFile = Path.GetFileName(application.CVLocation);
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", cvFile);
            var cvService = new CVService();
            var cvText = await cvService.ExtractTextFromPDFAsync(filePath);

            string quizAnswers = "Q1 " + application.MCQ1 + "\n" + "Q2 " + application.MCQ2 + "\n" + "Q3 " + application.MCQ3 + "\n" + "Q4 " + application.MCQ4 + "\n" + "Q5 " + application.OEQ1 + "\n" + "Q6 " + application.OEQ2 + "\n" + "Q7 " + application.OEQ3;

            var aiFeedback = await _aiService.GetSuitabilityScore(cvText, application.selectedRole, quizAnswers, interviewNotes);

            application.AIReviewSummary = aiFeedback.Summary;
            application.SuitabilityScore = aiFeedback.Score;
            application.Status = "Interview Completed";
            await _context.SaveChangesAsync();

            return RedirectToAction("ApplicationList");
        }

        [Authorize]
        public async Task<IActionResult> SendContract(int id)
        {
            var application = _context.EmploymentApplications.Where(e => e.id == id).Select(e => e).SingleOrDefault();

            var employmentlink = Url.Action("FinalizeEmployment", "Employment", new { id = application.id }, Request.Scheme);

            SmtpClient client = new SmtpClient("Smtp.gmail.com");
            client.Port = 587;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("Hybridlearnacademy05@gmail.com", "utxw xbwc dsik yysx");
            MailAddress mailAddress = new MailAddress("Hybridlearnacademy05@gmail.com");
            MailAddress recipientAddress = new MailAddress(application.Email);
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = mailAddress;
            mailMessage.To.Add(recipientAddress);
            mailMessage.Subject = "HLA Job Application!";

            mailMessage.Body = $"Good day! \n\n Congratulations! We have decided to take you on as {application.selectedRole} at Hybrid Learning Academy! \n\n There is just one more step to be complete. \n Please sign the Employment Contract and upload required documents. Please refer to the link below: {employmentlink}\n\n ";

            application.Status = "Contract/Documents Pending";

            await client.SendMailAsync(mailMessage);

            return RedirectToAction("ApplicationList", "Employment");
        }

        public async Task<IActionResult> FinalizeEmployment(int id)
        {
            var application = _context.EmploymentApplications.Where(e => e.id == id).Select(e => e).SingleOrDefault();


            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> FinalizeEmployment(int id,
    IFormFile SignedContract,
    IFormFile IdCopy,
    IFormFile SaceCert,
    IFormFile PoliceClearance,
    IFormFile BankLetter)
        {
            var app = _context.EmploymentApplications.Find(id);
            if (app == null) return NotFound();

            
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads/StaffDocs", app.id.ToString());

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            
            if (SignedContract != null)
                app.ContractLocation = await SaveFile(SignedContract, folderPath, "Contract", app.id);

            if (IdCopy != null)
                app.IdCopyLocation = await SaveFile(IdCopy, folderPath, "ID", app.id);

            if (SaceCert != null)
                app.SACELocation = await SaveFile(SaceCert, folderPath, "SACE", app.id);

            if (PoliceClearance != null)
                app.PoliceClearanceLocation = await SaveFile(PoliceClearance, folderPath, "Police", app.id);

            if (BankLetter != null)
                app.BankLetterLocation = await SaveFile(BankLetter, folderPath, "Bank", app.id);

            app.Status = "Contract/Documents Uploaded. \n Awaiting Verification";
            _context.Update(app);
            await _context.SaveChangesAsync();

            return RedirectToAction("ApplicationList");
        }

        private async Task<string> SaveFile(IFormFile file, string folderPath, string docType, int appId)
        {
            if (file == null || file.Length == 0) return null;

            string fileName = $"{appId}_{docType}{Path.GetExtension(file.FileName)}";
            string fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/Uploads/StaffDocs/{appId}/{fileName}";
        }


        public IActionResult DownloadContract(int id)
        {
            var pdf = _employmentService.GetEmploymentContract(id);

            var stream = new MemoryStream();
            pdf.Save(stream, false);
            return File(stream.ToArray(), "application/pdf", "EmploymentContract.pdf");
        }

        [Authorize]
        public async Task<IActionResult> AcceptApplicant(int id)
        {
            var application = _context.EmploymentApplications.Where(e => e.id == id).Select(e => e).SingleOrDefault();

            string callbackUrl = Url.Action("CreateStaffAccount", "Account", new { email = application.Email }, protocol: Request.Scheme);
            SmtpClient client = new SmtpClient("Smtp.gmail.com");
            client.Port = 587;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("Hybridlearnacademy05@gmail.com", "utxw xbwc dsik yysx");
            MailAddress mailAddress = new MailAddress("Hybridlearnacademy05@gmail.com");
            MailAddress recipientAddress = new MailAddress(application.Email);
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = mailAddress;
            mailMessage.To.Add(recipientAddress);
            mailMessage.Subject = "HLA Job Application!";

            mailMessage.Body = $"Good Day! \n\n" +
                   $"Your documents have all been processed and verified. \n" +
                   $"You are expected to start work from next week Monday. \n\n" +
                   $"Please ensure you create your staff account here: \n" +
                   $"{callbackUrl} \n\n" +
                   $"Welcome to the team!";

            application.Status = "Awaiting Account Creation";
            await _context.SaveChangesAsync();

            await client.SendMailAsync(mailMessage);

            return RedirectToAction("ApplicationList");
            
        }

    }
}
