using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Services
{
    public class WeeklyReportWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _targetEmail = "ssshaik939@gmail.com"; // Set the recipient here

        public WeeklyReportWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                // Check if it's Monday morning at 8 AM
                if (now.DayOfWeek == DayOfWeek.Monday && now.Hour == 8)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                        var reportService = scope.ServiceProvider.GetRequiredService<AttendanceReportService>();

                        // 1. Logic to fetch the data for the model
                        var model = await GetAttendanceData(context);

                        // 2. Generate PDF
                        var pdfBytes = reportService.GenerateWeeklyReportPdf(model);

                        // 3. Email PDF
                        await reportService.SendWeeklyEmail(_targetEmail, pdfBytes);
                    }

                    // Wait 1 hour so we don't send multiple emails in the same hour
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }

                // Check every hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task<InsightsOnStudentAttendance> GetAttendanceData(ApplicationDBContext context)
        {
            var today = DateTime.Today;

            // 1. Basic Stats
            int total = await context.Students.CountAsync();

            // Counting students marked "Present" today
            int present = await context.Attendances
                .Where(a => a.Date == today && a.Status == "Present")
                .CountAsync();

            // 2. Identify Flagged Students (3+ absences)
            var flagged = await context.Attendances
                .Where(a => a.Status == "Absent")
                .GroupBy(a => new { a.Student.StudentId, a.Student.FullName }) // Using navigation property
                .Select(g => new FlaggedStudent
                {
                    StudentId = g.Key.StudentId,
                    Name = g.Key.FullName,
                    TotalAbsences = g.Count()
                })
                .Where(s => s.TotalAbsences >= 3)
                .ToListAsync();

            // 3. Assemble the Insight Model
            return new InsightsOnStudentAttendance
            {
                TotalStudents = total,
                PresentToday = present,
                DailyAttendanceRate = total > 0 ? Math.Round(((double)present / total) * 100, 1) : 0,
                AtRiskStudents = flagged
            };
        }
    }
}