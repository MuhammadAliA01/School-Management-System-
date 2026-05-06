using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models; // Your namespace
using System.Linq;

namespace SchoolManagmentSystem.Controllers
{
    public class EventController : Controller
    {
        // 1. Set up your Database Context
        // Note: Change 'ApplicationDbContext' to whatever your context class is actually called!
        private readonly ApplicationDBContext _context;

        public EventController(ApplicationDBContext context)
        {
            _context = context;
        }

        // 2. The Index Action (Shows the list of events)
        public IActionResult Index()
        {
            // Go to the database, grab every event in the EventCalendars table, and turn it into a list
            var events = _context.EventCalendars.ToList();

            // Pass that list to the View
            return View(events);
        }
        // 3. Shows the specific event for purchasing tickets
        public IActionResult Purchase(int id)
        {
            // Find the specific event in the database
            var selectedEvent = _context.EventCalendars.FirstOrDefault(e => e.Id == id);

            if (selectedEvent == null)
            {
                return NotFound();
            }

            // Set up pricing rules for the view
            ViewBag.TicketPrice = 150.00m;
            ViewBag.FreeTicketsAllowed = 2;

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
