using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Services
{
    public class PayslipGenerationService
    {
        private readonly ApplicationDBContext _context;
        private readonly PayslipPdfService _pdfService;
        // test pass for dev. NB change to false in production
        private const bool UseTestPassword = true;
        private const string TestPassword = "1234";
        public PayslipGenerationService(ApplicationDBContext context, PayslipPdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        // regenerationReason if existing payslip is found
        public Payslip StagePayslip(PayslipDTO data, string? regenerationReason = null)
        {
            var rawPdfBytes = _pdfService.BuildPdf(data);
            var password = UseTestPassword ? TestPassword : data.IDNumber;
            var pdfBytes = _pdfService.ProtectPdf(rawPdfBytes, password); //

            var existing = _context.Payslips.FirstOrDefault(p =>
                p.StaffId == data.StaffId &&
                p.PayPeriodStart == data.PayPeriodStart &&
                p.PayPeriodEnd == data.PayPeriodEnd);

            if (existing != null)
            {
                if (string.IsNullOrWhiteSpace(regenerationReason))
                    throw new InvalidOperationException("A reason is required to regenerate an existing payslip.");

                // archive the old version
                var archived = new PayslipVersionControl
                {
                    PayslipId = existing.PayslipId,
                    ArchivedPdfContent = existing.PdfContent,
                    ArchivedVersionNumber = existing.CurrentVersion,
                    RegenerationReason = regenerationReason,
                    ArchivedAt = DateTime.Now
                };
                _context.PayslipVersionControls.Add(archived);

                // update existing payslip with new content
                existing.PdfContent = pdfBytes;
                existing.CurrentVersion += 1;
                existing.Status = PayslipStatus.PendingConfirmation;
                existing.FailureReason = null;

                _context.SaveChanges();
                return existing;
            }

            // no existing payslip 
            var payslip = new Payslip
            {
                StaffId = data.StaffId,
                PayPeriodStart = data.PayPeriodStart,
                PayPeriodEnd = data.PayPeriodEnd,
                PdfContent = pdfBytes,
                Status = PayslipStatus.PendingConfirmation,
                CurrentVersion = 1
            };

            _context.Payslips.Add(payslip);
            _context.SaveChanges();

            return payslip;
        }
        // Confirm Distribution step completes
        public void FinalizePayslip(int payslipId)
        {
            var payslip = _context.Payslips.Find(payslipId);
            if (payslip == null) return;

            payslip.Status = PayslipStatus.Generated;
            payslip.GeneratedAt = DateTime.Now;

            _context.SaveChanges();
        }
    }
}