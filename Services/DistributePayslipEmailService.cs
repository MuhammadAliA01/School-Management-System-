using System.Net;
using System.Net.Mail;

namespace SchoolManagmentSystem.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string toEmail,
            string subject,
            string body);

        Task SendEmailAsync(
            string toEmail,
            string subject,
            string body,
            byte[] attachment,
            string attachmentName);
    }

    public class DistributePayslipEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public DistributePayslipEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body)
        {
            var smtpServer =
                _configuration["EmailSettings:SmtpServer"];

            var port =
                int.Parse(
                    _configuration["EmailSettings:Port"] ?? "587");

            var enableSsl =
                bool.Parse(
                    _configuration["EmailSettings:EnableSsl"] ?? "true");

            var fromEmail =
                _configuration["EmailSettings:FromEmail"];

            var fromName =
                _configuration["EmailSettings:FromName"];

            var username =
                _configuration["EmailSettings:Username"];

            var password =
                _configuration["EmailSettings:Password"];

            using var client = new SmtpClient(smtpServer, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(
                    username,
                    password)
            };

            using var message = new MailMessage
            {
                From = new MailAddress(
                    fromEmail!,
                    fromName),

                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            await client.SendMailAsync(message);
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body,
            byte[] attachment,
            string attachmentName)
        {
            var smtpServer =
                _configuration["EmailSettings:SmtpServer"];

            var port =
                int.Parse(
                    _configuration["EmailSettings:Port"] ?? "587");

            var enableSsl =
                bool.Parse(
                    _configuration["EmailSettings:EnableSsl"] ?? "true");

            var fromEmail =
                _configuration["EmailSettings:FromEmail"];

            var fromName =
                _configuration["EmailSettings:FromName"];

            var username =
                _configuration["EmailSettings:Username"];

            var password =
                _configuration["EmailSettings:Password"];

            using var client = new SmtpClient(smtpServer, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(
                    username,
                    password)
            };

            using var message = new MailMessage
            {
                From = new MailAddress(
                    fromEmail!,
                    fromName),

                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            using var attachmentStream =
                new MemoryStream(attachment);

            var pdfAttachment = new Attachment(
                attachmentStream,
                attachmentName,
                "application/pdf");

            message.Attachments.Add(pdfAttachment);

            await client.SendMailAsync(message);
        }
    }
}