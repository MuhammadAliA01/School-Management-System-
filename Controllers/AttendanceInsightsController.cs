using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;
using Microsoft.EntityFrameworkCore;

// using SchoolManagmentSystem.Data; // Uncomment if it gives a red line under ApplicationDbContext
using System;
using System.Linq;

namespace SchoolManagmentSystem.Controllers
{
    public class AttendanceInsightsController : Controller
    {
        private readonly ApplicationDBContext manager;
        private readonly AttendanceReportService _reportService;

        public AttendanceInsightsController(ApplicationDBContext dbcontext, AttendanceReportService reportService)
        {
            manager = dbcontext;
            _reportService = reportService;
        }

        [HttpGet("test-pdf-real")] // this is for testing the pdf to see if it generate without waiting a wjole week
        public async Task<IActionResult> DownloadRealPdf()
        {
            var today = DateTime.Today;

            int total = await manager.Students.CountAsync();

            int present = await manager.Attendances
                .Where(a => a.Date.Date == today && a.Status == "Present")
                .CountAsync();

            var flagged = await manager.Attendances
                .Where(a => a.Status == "Absent")
                .GroupBy(a => new { a.Student.StudentId, a.Student.FullName }) 
                .Select(g => new FlaggedStudent
                {
                    StudentId = g.Key.StudentId,
                    Name = g.Key.FullName,
                    TotalAbsences = g.Count()
                })
                .Where(s => s.TotalAbsences >= 3)
                .ToListAsync();

            var realModel = new InsightsOnStudentAttendance
            {
                TotalStudents = total,
                PresentToday = present,
                DailyAttendanceRate = total > 0 ? Math.Round(((double)present / total) * 100, 1) : 0,
                AtRiskStudents = flagged
            };

            byte[] pdfBytes = _reportService.GenerateWeeklyReportPdf(realModel);

            return File(pdfBytes, "application/pdf", "RealAttendanceReport.pdf");
        }

        [HttpGet("test-email-send")] // this is for testing to see if email send
        public async Task<IActionResult> TestEmailSend(string recipient = "your-own-email@gmail.com")
        {
            try
            {
                
                var today = DateTime.Today;
                var realModel = new InsightsOnStudentAttendance
                {
                    TotalStudents = await manager.Students.CountAsync(),
                    PresentToday = await manager.Attendances
                        .Where(a => a.Date.Date == today && a.Status == "Present").CountAsync(),
                    DailyAttendanceRate = 0, 
                    AtRiskStudents = await manager.Attendances
                        .Where(a => a.Status == "Absent")
                        .GroupBy(a => new { a.Student.StudentId, a.Student.FullName })
                        .Select(g => new FlaggedStudent { Name = g.Key.FullName, TotalAbsences = g.Count() })
                        .Where(s => s.TotalAbsences >= 3).ToListAsync()
                };

                byte[] pdfBytes = _reportService.GenerateWeeklyReportPdf(realModel);

                await _reportService.SendWeeklyEmail(recipient, pdfBytes);

                return Ok($"Success! The attendance report was sent to {recipient}. Check your inbox (and spam folder!).");
            }
            catch (Exception ex)
            {
                return BadRequest($"Email failed: {ex.Message}");
            }
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