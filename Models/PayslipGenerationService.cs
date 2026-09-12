using SchoolManagmentSystem.Models;
namespace SchoolManagmentSystem.Services
{
    public class PayslipGenerationService
    {
        private readonly ApplicationDBContext _context;
        private readonly PayslipPdfService _pdfService;

        public PayslipGenerationService(ApplicationDBContext context, PayslipPdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        // Admin triggers this — stages the payslip
        public Payslip StagePayslip(PayslipDTO data)
        {
            var pdfBytes = _pdfService.BuildPdf(data);

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