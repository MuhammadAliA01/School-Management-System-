using Microsoft.AspNetCore.Mvc;

namespace SchoolManagmentSystem.Controllers
{
    public class DashboardsController : Controller
    {
        public IActionResult ParentDashboard()
        {
            return View();
        }

        public IActionResult StaffDashboard()
        {
            return View();
        }
    }
}
