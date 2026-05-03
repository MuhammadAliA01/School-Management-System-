using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
// using SchoolManagmentSystem.Data; // Uncomment this if it gives you a red line under ApplicationDbContext
using System;
using System.Linq;

namespace SchoolManagmentSystem.Controllers
{
    public class AttendanceController : Controller
    {
        // 1. Bring in your database manager
        private readonly ApplicationDBContext _context;

        public AttendanceController(ApplicationDBContext Context)
        {
            _context = Context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var pageData = new MorningAttendanceData();

            //gets students from db, stores in dbstudents
            var dbStudents = _context.Students.ToList();

            // Loop through them and put them on the screen
            foreach (var kid in dbStudents)
            {
                pageData.Students.Add(new StudentRow
                {
                    StudentId = kid.StudentId,
                    Name = kid.FullName,
                    Status = "Present" // saves time by defaulting to "Present" for every student, but the teacher can change it on the screen
                });
            }

            return View(pageData);
        }

       
        [HttpPost]
        public IActionResult Index(MorningAttendanceData pageData)
        {
            if (ModelState.IsValid)
            {
                // Loop through every kid the teacher just submitted on the screen
                foreach (var studentRow in pageData.Students)
                {
                    var newRecord = new Attendance
                    {
                        StudentId = studentRow.StudentId,
                        Date = pageData.Date,
                        Status = studentRow.Status
                    };

                    // Add them to the save queue
                    _context.Attendances.Add(newRecord);
                }

                //idk why but it won't save to the database until you call this method, which processes the whole save queue at once
                _context.SaveChanges();


                TempData["SuccessMessage"] = "Attendance securely saved to the database!";
                return RedirectToAction("Index");
            }

            return View(pageData);
        }
    }
}