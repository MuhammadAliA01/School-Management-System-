using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models; // Your namespace
using System.Linq;

namespace SchoolManagmentSystem.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly string _apiKey = "YOUR_GEMINI_API_KEY"; // Get from Google AI Studio

        {
        }

        {
        }
        {

            {
            }


            return View(selectedEvent);
        }

        // 4. Processes the submitted order securely
        [HttpPost]
        public IActionResult ProcessOrder(int EventId, int quantity)
        {
            decimal ticketPrice = 150.00m;
            int freeTicketsAllowed = 2;

            // Backend calculation: always recalculate on the server for security
            int billableTickets = quantity - freeTicketsAllowed;
            if (billableTickets < 0) billableTickets = 0;

            decimal finalTotalCost = billableTickets * ticketPrice;

            if (finalTotalCost == 0)
            {
                // Free transaction - bypass payment
                TempData["SuccessMessage"] = $"Successfully claimed {quantity} free ticket(s)!";
                return RedirectToAction("Index");
            }
            else
            {
                // Paid transaction - proceed to the checkout controller (we will build this in Step 5)
                return RedirectToAction("Checkout", "PurchaseTicket", new
                {
                    eventId = EventId,
                    quantity = quantity,
                    amount = finalTotalCost
                });
            }
        }
    }
}
