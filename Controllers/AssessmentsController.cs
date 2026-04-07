using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
using System;
using System.Linq;

namespace SchoolManagmentSystem.Controllers
{
    [Authorize(Roles = "Staff")] 
    public class AssessmentsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public AssessmentsController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult LogGrade(int studentId)
        {
            ViewBag.StudentId = studentId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogGrade(StudentAssessment assessment)
        {
            var staffIdStr = User.FindFirst("UserInternalID")?.Value;
            if (int.TryParse(staffIdStr, out int staffId))
            {
                assessment.TeacherId = staffId;
            }

            assessment.AssessmentDate = DateTime.Now;

            _context.StudentAssessments.Add(assessment);
            _context.SaveChanges();

            return RedirectToAction("Dashboard", "TeachingMethods");
        }
    }
}