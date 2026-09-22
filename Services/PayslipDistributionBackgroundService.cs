using SchoolManagmentSystem.Models;

using Microsoft.EntityFrameworkCore;


namespace SchoolManagmentSystem.Services
{
    public class PayslipDistributionBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PayslipDistributionBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider
                        .GetRequiredService<ApplicationDBContext>();

                    var emailService = scope.ServiceProvider
                        .GetRequiredService<IEmailService>();

                    var pendingDistributions = await context.PayslipDistributions
                        .Where(d => d.Status == DistributionStatus.Pending &&
                                    d.ScheduledFor <= DateTime.Now)
                        .ToListAsync(stoppingToken);

                    foreach (var distribution in pendingDistributions)
                    {
                        var staff = await context.Staffs
                            .FirstOrDefaultAsync(
                                s => s.id == distribution.StaffId,
                                stoppingToken);

                        var payslip = await context.Payslips
                            .FirstOrDefaultAsync(
                                p => p.PayslipId == distribution.PayslipId,
                                stoppingToken);

                        if (staff == null || payslip == null)
                        {
                            distribution.Status = DistributionStatus.Failed;
                            distribution.FailureReason =
                                "Employee or payslip could not be found.";

                            continue;
                        }

                        try
                        {
                            var payslipLink =
                            $"https://localhost:7206/PayslipDistribution/ViewPayslip?token={distribution.AccessToken}";

                            var emailBody = $@"
                                <p>Dear {staff.FirstName} {staff.Surname},</p>

                                <p>Your payslip is now available.</p>

                                <p>
                                    <a href=""{payslipLink}"">
                                        View Your Payslip
                                    </a>
                                </p>

                                <p>You can view your payslip using the link above.
                                If you are not currently logged in, you will be
                                asked to log in first.</p>

                                <p>Regards,<br>
                                Hybrid Learning Academy</p>
                            ";

                            if (payslip.PdfContent == null || payslip.PdfContent.Length == 0)
                            {
                                distribution.Status = DistributionStatus.Failed;
                                distribution.FailureReason = "Payslip PDF is missing.";
                                continue;
                            }

                            await emailService.SendEmailAsync(
                                staff.Email,
                                "Your Payslip - Hybrid Learning Academy",
                                emailBody,
                                payslip.PdfContent,
                                $"Payslip_{staff.FirstName}_{staff.Surname}_{payslip.PayPeriodStart:yyyyMM}.pdf");

                            distribution.DistributedAt = DateTime.Now;
                            distribution.Status = DistributionStatus.Distributed;
                        }
                        catch (Exception ex)
                        {
                            distribution.Status = DistributionStatus.Failed;
                            distribution.FailureReason = ex.Message;
                        }
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}