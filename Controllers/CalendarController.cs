using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mscc.GenerativeAI;
using SchoolManagmentSystem.Models;
using Microsoft.Extensions.AI;


namespace SchoolManagmentSystem.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly ApplicationDBContext _context;
        private readonly Microsoft.Extensions.AI.IChatClient? _aiClient; // Added ? to make it optional

        public CalendarController(ApplicationDBContext context, Microsoft.Extensions.AI.IChatClient? aiClient = null)
        {
            _context = context;
            _aiClient = aiClient; 
        }

        [HttpGet("events")]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.EventCalendars.ToListAsync();

            var calendarData = events.Select(e => new {
                id = e.Id,
                title = e.Title, // FullCalendar needs lowercase 'title'
                start = e.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"), // FullCalendar needs lowercase 'start'
                end = e.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                description = e.Desc,
                location = e.Location,
                attendeeCount = e.AttendeeCount
            });

            return Json(calendarData); // Use return Json() instead of Ok()
        }



        [HttpPost("rsvp/{id}")]
        public async Task<IActionResult> RSVP(int id)
        {
            var ev = await _context.EventCalendars.FindAsync(id);
            if (ev == null) return NotFound();
            ev.AttendeeCount++;
            await _context.SaveChangesAsync();
            return Ok(ev.AttendeeCount);
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            var ev = await _context.EventCalendars.FindAsync(id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventCalendar ev)
        {
            if (ModelState.IsValid)
            {
                _context.Update(ev);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Manage));
            }
            return View(ev);
        }
        
        public async Task<IActionResult> Manage()
        {
            var events = await _context.EventCalendars.OrderBy(e => e.StartTime).ToListAsync();
            return View(events);
        }

        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 3. POST: Saving the Event
        [HttpPost]
        public async Task<IActionResult> Create(EventCalendar ev)
        {
            if (ModelState.IsValid)
            {
                _context.EventCalendars.Add(ev);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Manage));
            }
            return View(ev);
        }


        
        [HttpPost]
public async Task<IActionResult> Delete(int id)
{
    var ev = await _context.EventCalendars.FindAsync(id);
    if (ev != null)
    {
        _context.EventCalendars.Remove(ev);
        await _context.SaveChangesAsync();
    }
    return RedirectToAction(nameof(Manage));
}

        















    }
}
