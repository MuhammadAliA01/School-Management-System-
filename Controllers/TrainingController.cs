using Microsoft.AspNetCore.Mvc;

namespace Training.Controllers
{
    public class TrainingController : Controller
    {

        public IActionResult TrainingDashboard()
        {
            return View();
        }

        public IActionResult TrainingLogGrade()
        {
            return View();
        }

        public IActionResult TrainingIncidentReport()
        {
            return View();
        }

        public IActionResult TrainingViewTeachingMethod()
        {
            return View();
        }

        public IActionResult TrainingGenerateMethod()
        {
            return View();
        }

    }
}
