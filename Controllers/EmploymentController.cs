using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using System.Linq;

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

        public IActionResult SearchStatus(string searchString)
        {
            var applicationstatus = _context.EmploymentApplications.Where(a => a.referenceNumber == searchString)
               .Select(a => a.Status).FirstOrDefault();

            ViewBag.Status = applicationstatus;


            return View();
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
                employmentApplication.Status = "Pending";
                employmentApplication.referenceNumber = refNumber;
                _context.Add(employmentApplication);
                await _context.SaveChangesAsync();
                return RedirectToAction("ApplicationConfirmed", "Employment", new {referenceNum = employmentApplication.referenceNumber});
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
