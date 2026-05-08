using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

            var ParentID = Convert.ToInt32(User.FindFirst("UserInternalID")?.Value);


            var ticketOwned = _context.ticketPurchases.Where(t => t.ParentID == ParentID && t.EventID == id).Select(t => t.ticketsOwned).FirstOrDefault();
            ViewBag.TicketsOwned = ticketOwned;

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

            var dbEvent = _context.EventCalendars
                .FirstOrDefault(e => e.Id == EventId);

            if (dbEvent == null)
            {
                return NotFound();
            }

            int parentId = Convert.ToInt32(
                User.FindFirst("UserInternalID")?.Value);

            var existingPurchase = _context.ticketPurchases
                .FirstOrDefault(t =>
                    t.ParentID == parentId &&
                    t.EventID == EventId);

            int ticketsOwned = existingPurchase?.ticketsOwned ?? 0;

            int remainingFreeTickets =
                freeTicketsAllowed - ticketsOwned;

            if (remainingFreeTickets < 0)
                remainingFreeTickets = 0;

            int billableTickets =
                quantity - remainingFreeTickets;

            if (billableTickets < 0)
                billableTickets = 0;

            decimal finalTotalCost =
                billableTickets * ticketPrice;

            // FREE CHECKOUT
            if (finalTotalCost == 0)
            {
                dbEvent.AttendeeCount += quantity;

                if (existingPurchase != null)
                {
                    existingPurchase.ticketsOwned += quantity;
                }
                else
                {
                    var newPurchase = new TicketPurchases
                    {
                        EventID = EventId,
                        ParentID = parentId,
                        ticketsOwned = quantity
                    };

                    _context.ticketPurchases.Add(newPurchase);
                }

                _context.SaveChanges();

                TempData["SuccessMessage"] =
                    $"Successfully claimed {quantity} free ticket(s)!";

                return RedirectToAction("Index");
            }

            // PAID CHECKOUT
            return RedirectToAction(
                "ProcessStripePayment",
                "PurchaseTicket",
                new
                {
                    eventId = EventId,
                    quantity = quantity
                });
        }
    }
}