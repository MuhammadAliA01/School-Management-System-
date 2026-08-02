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
    $"{Request.Scheme}://{Request.Host}/PurchaseTicket/Success?session_id={{CHECKOUT_SESSION_ID}}",

                CancelUrl =
    $"{Request.Scheme}://{Request.Host}/PurchaseTicket/Cancel",

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

            decimal amountPaid =
    session.AmountTotal.GetValueOrDefault() / 100m;



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

            var parentemail = _context.Parents.Where(p => p.Id == parentId).Select(p => p.Email).FirstOrDefault();

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

            // 4. Generate Reference Number & Get Email
            string referenceNumber = "TKT-" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            string parentEmail = parentemail ?? "ssshaik939@gmail.com";

            // 5. Send Confirmation Email (Using your SmtpClient logic)
            try
            {
                using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
                {
                    client.Port = 587;
                    client.EnableSsl = true;

                    // PASTE YOUR APP PASSWORD HERE
                    client.Credentials = new NetworkCredential("Hybridlearnacademy05@gmail.com", "utxw xbwc dsik yysx");

                    MailAddress mailAddress = new MailAddress("Hybridlearnacademy05@gmail.com");
                    MailAddress recipientAddress = new MailAddress(parentEmail);

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = mailAddress;
                    mailMessage.To.Add(recipientAddress);

                    mailMessage.Subject = $"Booking Confirmation: {dbEvent.Title}";
                    mailMessage.Body = $"Dear {User.Identity.Name},\n\n" +
                                       $"Thank you for your purchase! Your tickets for '{dbEvent.Title}' are confirmed.\n\n" +
                                       $"Reference Number: {referenceNumber}\n" +
                                       $"Tickets Booked: {quantity}\n" +
                                       $"Amount Paid: R {amountPaid:F2}\n\n" +
                                       $"Please keep this reference number handy for check-in at the event.\n\n" +
                                       $"Regards,\nHLA Administration";

                    client.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed to send: {ex.Message}");
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