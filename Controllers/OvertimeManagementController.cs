using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers
{
    public class OvertimeManagementController : Controller
    {
        private readonly ApplicationDBContext _context;

        public OvertimeManagementController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var requests = await _context.OvertimeRequests
                .OrderByDescending(x => x.DateSubmitted)
                .ToListAsync();

            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var request = await _context.OvertimeRequests
                .FirstOrDefaultAsync(x => x.OvertimeId == id);

            if (request == null)
            {
                TempData["Error"] = "Overtime request not found.";
                return RedirectToAction("Index");
            }

            if (request.Status != "Pending")
            {
                TempData["Error"] = "Only pending overtime requests can be accepted.";
                return RedirectToAction("Index");
            }

            request.Status = "Accepted";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Overtime request accepted successfully.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var request = await _context.OvertimeRequests
                .FirstOrDefaultAsync(x => x.OvertimeId == id);

            if (request == null)
            {
                TempData["Error"] = "Overtime request not found.";
                return RedirectToAction("Index");
            }

            if (request.Status != "Pending")
            {
                TempData["Error"] = "Only pending overtime requests can be rejected.";
                return RedirectToAction("Index");
            }

            request.Status = "Rejected";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Overtime request rejected successfully.";

            return RedirectToAction("Index");
        }
    }
}