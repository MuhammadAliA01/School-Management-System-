using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
using Stripe; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolManagmentSystem.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public PaymentsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PayDeposit(int id)
        {
            var registration = await _context.StudentRegistrations.FindAsync(id);
            if (registration == null) return BadRequest("Application not found.");

            var domain = "https://localhost:44302/";

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
                {
                    new Stripe.Checkout.SessionLineItemOptions
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            UnitAmount = 500000, // R5000 in cents
                            Currency = "zar",
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Admission Deposit",
                                Description = $"Enrollment Deposit for {registration.ChildName}",
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = domain + $"Payments/PaymentSuccess?id={id}",
                CancelUrl = domain + $"Dashboards/ParentDashboard?parentID={registration.ParentId}",
            };

            var service = new Stripe.Checkout.SessionService();
            var session = await service.CreateAsync(options);

            registration.StripeSessionId = session.Id;
            await _context.SaveChangesAsync();

            return Redirect(session.Url);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentSuccess(int id)
        {
            var registration = await _context.StudentRegistrations.FindAsync(id);

            if (registration != null)
            {
                registration.feesPaid = true;
                await _context.SaveChangesAsync();

                return RedirectToAction("ParentDashboard", "Dashboards", new { parentID = registration.ParentId });
            }

            return RedirectToAction("Index", "Home");
        }
    }
}