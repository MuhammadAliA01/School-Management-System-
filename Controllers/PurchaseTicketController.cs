using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
using System;
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

        [HttpPost]
        public IActionResult ProcessStripePayment(int eventId, int quantity, decimal amount, string cardHolderName)
        {
            bool isPaymentSuccessful = true; // Simulated Stripe success

            if (isPaymentSuccessful)
            {
                // 1. Get the logged-in Parent's ID
                var parentIdString = User.FindFirst("UserInternalID")?.Value;
                int parentId = 0;
                int.TryParse(parentIdString, out parentId);

                var dbEvent = _context.EventCalendars.FirstOrDefault(e => e.Id == eventId);

                if (dbEvent != null)
                {
                    // 2. Update the total attendee count
                    dbEvent.AttendeeCount += quantity;

                    // 3. Save the official Booking Record
                    if (parentId > 0)
                    {
                        var newBooking = new TicketBooking
                        {
                            EventId = eventId,
                            ParentId = parentId,
                            Quantity = quantity,
                            AmountPaid = amount,
                            PurchaseDate = DateTime.Now
                        };
                        _context.TicketBookings.Add(newBooking);
                    }

                    _context.SaveChanges();

                    // 4. Generate Reference Number & Get Email
                    string referenceNumber = "TKT-" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
                    string parentEmail = User.Identity?.Name ?? "ismailrazak18@gmail.com";

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
                            mailMessage.Body = $"Dear {cardHolderName},\n\n" +
                                               $"Thank you for your purchase! Your tickets for '{dbEvent.Title}' are confirmed.\n\n" +
                                               $"Reference Number: {referenceNumber}\n" +
                                               $"Tickets Booked: {quantity}\n" +
                                               $"Amount Paid: R {amount:F2}\n\n" +
                                               $"Please keep this reference number handy for check-in at the event.\n\n" +
                                               $"Regards,\nHLA Administration";

                            client.Send(mailMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Email failed to send: {ex.Message}");
                    }

                    // 6. UI Success Message
                    TempData["SuccessMessage"] = $"Purchase of {quantity} ticket(s) confirmed! Reference: {referenceNumber}. A receipt has been emailed to {parentEmail}.";

                    return RedirectToAction("Index", "Event");
                }
            }

            return View("Checkout");
        }
    }
}