using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
using Microsoft.EntityFrameworkCore;
using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Types;
using System.Collections.Generic;

namespace SchoolManagmentSystem.Controllers
{
    public class EventController : Controller
    {

        private readonly ApplicationDBContext _context;
        private readonly string _apiKey = "YOUR_GEMINI_API_KEY"; // Get from Google AI Studio

       
        public EventController(ApplicationDBContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.Include(e => e.RSVPs).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> GenerateDescription(string title)
        {
            var googleAI = new GoogleAI(_apiKey);
            var model = googleAI.GenerativeModel(Model.Gemini25Flash);
            var response = await model.GenerateContent($"Write a fun description for an event: {title}");
            return Json(new { description = response.Text });
        }

        [HttpPost]
        public async Task<IActionResult> RSVP(int eventId, string userName, bool attending)
        {
            var rsvp = new RSVP { EventId = eventId, UserName = userName, IsAttending = attending };
            _context.RSVPs.Add(rsvp);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<JsonResult> GetCalendarEvents()
        {
            var events = await _context.Events.Select(e => new {
                id = e.EventId,
                title = e.Title,
                start = e.EventDate.ToString("yyyy-MM-dd"),
                description = e.Description
            }).ToListAsync();

            return Json(events);
        }

        // Standard CRUD (Create, Edit, Delete) logic here...



























    }
}
