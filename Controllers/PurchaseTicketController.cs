using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
using System.Linq;
using Stripe.Checkout;

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
            decimal ticketPrice = 150m;

            int parentId = Convert.ToInt32(
                User.FindFirst("UserInternalID")?.Value);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
        {
            "card"
        },

                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "zar",

                    ProductData =
                        new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "School Event Ticket"
                        },

                    UnitAmount = (long)(ticketPrice * 100)
                },

                Quantity = quantity
            }
        },

                Mode = "payment",

                SuccessUrl =
                    $"https://localhost:7205/PurchaseTicket/Success?session_id={{CHECKOUT_SESSION_ID}}",

                CancelUrl =
                    $"https://localhost:7205/PurchaseTicket/Cancel",

                Metadata = new Dictionary<string, string>
        {
            { "EventId", eventId.ToString() },
            { "Quantity", quantity.ToString() },
            { "ParentId", parentId.ToString() }
        }
            };

            var service = new SessionService();

            Session session = service.Create(options);

            return Redirect(session.Url);
        }



        [HttpGet]
        public IActionResult Success(string session_id)
        {
            var stripeService = new SessionService();

            var session = stripeService.Get(session_id);

            // Verify payment succeeded
            if (session.PaymentStatus != "paid")
            {
                TempData["ErrorMessage"] = "Payment failed.";

                return RedirectToAction("Index", "Calendar");
            }

            int eventId =
                Convert.ToInt32(session.Metadata["EventId"]);

            int quantity =
                Convert.ToInt32(session.Metadata["Quantity"]);

            int parentId =
                Convert.ToInt32(session.Metadata["ParentId"]);

            var dbEvent = _context.EventCalendars
                .FirstOrDefault(e => e.Id == eventId);

            if (dbEvent == null)
            {
                return NotFound();
            }

            var existingPurchase = _context.ticketPurchases
                .FirstOrDefault(t =>
                    t.ParentID == parentId &&
                    t.EventID == eventId);

            // SAVE TICKETS ONLY AFTER PAYMENT
            dbEvent.AttendeeCount += quantity;

            if (existingPurchase != null)
            {
                existingPurchase.ticketsOwned += quantity;
            }
            else
            {
                var purchase = new TicketPurchases
                {
                    EventID = eventId,
                    ParentID = parentId,
                    ticketsOwned = quantity
                };

                _context.ticketPurchases.Add(purchase);
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] =
                "Payment successful and tickets booked!";

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