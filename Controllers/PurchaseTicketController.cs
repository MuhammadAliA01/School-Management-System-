using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
using Stripe.Checkout;
using Stripe.V2;
using System.Linq;
using System.Net;
using System.Net.Mail;

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

        [HttpGet]
        public IActionResult ProcessStripePayment(int eventId, int quantity)
        {
            // 1. Simulate Stripe Success
            bool isPaymentSuccessful = true;

            int parentId = Convert.ToInt32(
                User.FindFirst("UserInternalID")?.Value);

            var options = new SessionCreateOptions
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

        [HttpGet]
        public IActionResult Cancel()
        {
            TempData["ErrorMessage"] =
                "Payment was cancelled.";

            return RedirectToAction("Index", "Event");
        }
    }
}