using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using System.Diagnostics;

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

        public IActionResult RegForm()
        {
            return View();
        }


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
                return RedirectToAction("Confirmation", new { id = registration.Id });
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




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]




        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
