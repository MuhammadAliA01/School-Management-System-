using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers
{
    public class DashboardsController : Controller
    {
        private readonly ApplicationDBContext _context;
        public DashboardsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult ParentDashboard(int parentID, StudentRegistration sr)
        {

            var nonregstudent = _context.StudentRegistrations.Where(s => s.ParentId == parentID).Select(s => s).FirstOrDefault();

            var student = _context.Students.Where(s => s.ParentId == parentID).Select(s => s).FirstOrDefault();


            bool nostudent = false;
            bool studentnoVerify = false;
            bool studentNoFees = false;
            bool studentNoReg = false;
            bool studentRegistered = false;

            if (student != null)
            {
                nostudent = false;
                studentnoVerify = false;
                studentNoFees = false;
                studentNoReg = false;
                studentRegistered = true;
                ViewBag.studentID = student.StudentId;

            }
            else if (student == null)
            {
                if (nonregstudent == null)
                {
                    nostudent = true;
                    studentnoVerify = false;
                    studentNoFees = false;
                    studentNoReg = false;
                    studentRegistered = false;
                    HttpContext.Session.SetString("AccessnoStudent", "true");
                    HttpContext.Session.SetString("AccessnoVerify", "false");
                    HttpContext.Session.SetString("AccessnoFees", "false");
                    HttpContext.Session.SetString("AccessnonReg", "false");


                }
                else if (nonregstudent.Status == "Pending")
                {
                    nostudent = false;
                    studentnoVerify = true;
                    studentNoFees = false;
                    studentNoReg = false;
                    studentRegistered = false;
                    HttpContext.Session.SetString("AccessnoVerify", "true");
                    HttpContext.Session.SetString("AccessnoStudent", "false");
                    HttpContext.Session.SetString("AccessnoFees", "false");
                    HttpContext.Session.SetString("AccessnonReg", "false");

                }
                else if ((nonregstudent.feesPaid != true) && nonregstudent.Status == "Verified")
                {
                    nostudent = false;
                    studentnoVerify = false;
                    studentNoFees = true;
                    studentNoReg = false;
                    studentRegistered = false;
                    HttpContext.Session.SetString("AccessnoFees", "true");
                    HttpContext.Session.SetString("AccessnoVerify", "false");
                    HttpContext.Session.SetString("AccessnoStudent", "false");
                    HttpContext.Session.SetString("AccessnonReg", "false");

                }
                else if (nonregstudent.feesPaid == true && nonregstudent.Status == "Verified")
                {
                    nostudent = false;
                    studentnoVerify = false;
                    studentNoFees = false;
                    studentNoReg = true;
                    studentRegistered = false;
                    ViewBag.parentID = nonregstudent.ParentId;
                    HttpContext.Session.SetString("AccessnonReg", "true");
                    HttpContext.Session.SetString("AccessnoFees", "false");
                    HttpContext.Session.SetString("AccessnoVerify", "false");
                    HttpContext.Session.SetString("AccessnoStudent", "false");
                }
            }
            ViewBag.noStudent = nostudent;
            ViewBag.studentNoVerify = studentnoVerify;
            ViewBag.studentNoFees = studentNoFees;
            ViewBag.studentNoReg = studentNoReg;
            ViewBag.studentRegistered = studentRegistered;
            ViewBag.ParentID = parentID;
            return View(nonregstudent);
        }

        [Authorize]
        public IActionResult StaffDashboard()
        {
            var parentId = User.FindFirst("UserInternalID")?.Value;

            if (User.IsInRole("Parent"))
            {
                return RedirectToAction("ParentDashboard", "Dashboards", new { parentID = parentId });
            }
            return View();
        }

    }
}
