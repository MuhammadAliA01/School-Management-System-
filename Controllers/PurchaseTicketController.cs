using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
using System.Linq;

namespace SchoolManagmentSystem.Controllers
{
    public class PurchaseTicketController : Controller
    {
        private readonly ApplicationDBContext _context;

        public PurchaseTicketController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Checkout(int eventId, int quantity, decimal amount)
        {
            ViewBag.EventId = eventId;
            ViewBag.Quantity = quantity;
            ViewBag.Amount = amount;
            return View();
        }

        [HttpPost]
        public IActionResult ProcessStripePayment(int eventId, int quantity, decimal amount, string cardHolderName)
        {
            // 1. Simulate Stripe Success
            bool isPaymentSuccessful = true;

            if (isPaymentSuccessful)
            {
                // 2. Update the Database Attendee Count
                var dbEvent = _context.EventCalendars.FirstOrDefault(e => e.Id == eventId);

                if (dbEvent != null)
                {
                    // Add the new tickets to the existing count
                    dbEvent.AttendeeCount += quantity;
                    _context.SaveChanges();
                }

                // 3. Create the Confirmation Message
                // We use TempData so the message survives the redirect to the next page
                TempData["SuccessMessage"] = $"Thank you, {cardHolderName}! Your purchase of {quantity} ticket(s) for '{dbEvent?.Title}' is confirmed. R {amount:F2} has been processed via Stripe.";

                return RedirectToAction("Index", "Event");
            }

            return View("Checkout");
        }
    }
}