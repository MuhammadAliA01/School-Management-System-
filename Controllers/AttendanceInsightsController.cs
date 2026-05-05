using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
// using SchoolManagmentSystem.Data; // Uncomment if it gives a red line under ApplicationDbContext
using System;
using System.Linq;

namespace SchoolManagmentSystem.Controllers
{
    public class AttendanceInsightsController : Controller
    {
        private readonly ApplicationDBContext manager;

        public AttendanceInsightsController(ApplicationDBContext dbcontext)
        {
            manager = dbcontext;
        }

        public IActionResult Index()
        {
            var dashboardData = new InsightsOnStudentAttendance();
            var today = DateTime.Today;

            //  Get all students in the database
            
            var allStudents = manager.Students.ToList();
            dashboardData.TotalStudents = allStudents.Count;

            //  Check today's attendance specifically
            var currentdayAttendance = manager.Attendances
                .Where(a => a.Date == today).ToList();

            dashboardData.PresentToday = currentdayAttendance.Count(a => a.Status == "Present" || a.Status == "Late");

            // Calculate the daily percentage
            if (dashboardData.TotalStudents > 0)
            {
                dashboardData.DailyAttendanceRate = Math.Round(((double)dashboardData.PresentToday / dashboardData.TotalStudents) * 100, 1);
            }


            int dangerThreshold = 3; //we can make changes to this number to make it more or less strict about who gets flagged on the dashboard

            for (int i = 0; i < allStudents.Count; i++)
            {
                int absenceCount = manager.Attendances
                    .Count(a => a.StudentId == allStudents[i].StudentId && a.Status == "Absent");

                if (absenceCount >= dangerThreshold)
                {
                    dashboardData.AtRiskStudents.Add(new FlaggedStudent
                    {
                        StudentId = allStudents[i].StudentId,
                        Name = allStudents[i].FullName,
                        TotalAbsences = absenceCount
                    });
                }
            }

            dashboardData.AtRiskStudents = dashboardData.AtRiskStudents.
                OrderByDescending(x => x.TotalAbsences).ToList();

            return View(dashboardData);
        }
    }
}