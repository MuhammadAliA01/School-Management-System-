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
        private readonly IChatClient _aiClient;
        public CalendarController(ApplicationDBContext context,IChatClient ai_Client)
        {
            _context = context;
            _aiClient = ai_Client;
        }

        [HttpGet("events")]
        public async Task<IActionResult> GetEvents() => Ok(await _context.CalendarEvents.ToListAsync());

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] EventCalendar ev)
        {
            _context.CalendarEvents.Add(ev);
            await _context.SaveChangesAsync();
            return Ok(ev);
        }

        [HttpPost("rsvp/{id}")]
        public async Task<IActionResult> RSVP(int id)
        {
            var ev = await _context.CalendarEvents.FindAsync(id);
            if (ev == null) return NotFound();
            ev.AttendeeCount++;
            await _context.SaveChangesAsync();
            return Ok(ev.AttendeeCount);
        }
        // Add similar methods for Delete and Edit.

        















    }
}
