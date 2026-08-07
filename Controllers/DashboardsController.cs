using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            if (User.IsInRole("Staff"))
            {
                bool teacher = false;
                bool admin = false;
                bool principal = false;
                var LoggedInUser = _context.Staffs.Where(l => l.id == Convert.ToInt32(parentId)).Select(l => l).FirstOrDefault();

                if (LoggedInUser.Role == "Teacher")
                {
                    teacher = true;
                    admin = false;
                    principal = false;

                }
                else if (LoggedInUser.Role == "Admin")
                {
                    teacher = false;
                    admin = true;
                    principal = false;
                }
                else if (LoggedInUser.Role == "Principal")
                {
                    teacher = false;
                    admin = false;
                    principal = true;
                }

                ViewBag.teacher = teacher;
                ViewBag.admin = admin;
                ViewBag.principal = principal;
            }


            return View();
        }
        public async Task<IActionResult> StaffHoursSummary()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var summary = await _context.StaffAttendances
               
                .Where(a => a.TotalHoursWorked != null
                         && a.Date.Month == currentMonth
                         && a.Date.Year == currentYear)
                .GroupBy(a => a.StaffId)
                .Select(g => new StaffHoursViewModel
                {
                    StaffId = (int)g.Key,
                    TotalHours = (decimal)g.Sum(a => a.TotalHoursWorked ?? 0)
                })
                .ToListAsync();

            foreach (var s in summary)
            {
                var staff = await _context.Staffs.FindAsync(s.StaffId);
                if (staff != null)
                {
                    s.FirstName = staff.FirstName;
                    s.Surname = staff.Surname;
                }
            }

            
            ViewBag.CurrentMonthName = DateTime.Now.ToString("MMMM yyyy");

            return View(summary);
        }
    }
}
