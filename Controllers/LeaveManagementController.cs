using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagmentSystem.Controllers
{
    public class LeaveManagementController : Controller
    {
        private readonly ApplicationDBContext _context;

        public LeaveManagementController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: /LeaveManagement
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var leaveRequests = await _context.LeaveRequests
                .OrderByDescending(x => x.StartDate)
                .ToListAsync();

            return View(leaveRequests);
        }

        // POST: /LeaveManagement/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var leaveRequest = await _context.LeaveRequests
                .FirstOrDefaultAsync(x => x.Id == id);

            if (leaveRequest == null)
            {
                TempData["Error"] = "Leave request could not be found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                int requestedDays =
                    (leaveRequest.EndDate.Date -
                     leaveRequest.StartDate.Date).Days + 1;

                var balance = await _context.LeaveBalances
                    .FirstOrDefaultAsync(x =>
                        x.StaffId == leaveRequest.StaffId &&
                        x.LeaveType == leaveRequest.Type);

                if (balance == null)
                {
                    TempData["Error"] =
                        "No leave balance exists for this staff member.";

                    return RedirectToAction(nameof(Index));
                }

                int availableDays =
                    balance.TotalAllocatedDays -
                    balance.UsedDays;

                if (requestedDays > availableDays)
                {
                    TempData["Error"] =
                        $"Cannot approve this request. " +
                        $"Only {availableDays} days remain.";

                    return RedirectToAction(nameof(Index));
                }

                leaveRequest.Approve();

                balance.UsedDays += requestedDays;

                _context.LeaveRequests.Update(leaveRequest);
                _context.LeaveBalances.Update(balance);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Leave request approved successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /LeaveManagement/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var leaveRequest = await _context.LeaveRequests
                .FirstOrDefaultAsync(x => x.Id == id);

            if (leaveRequest == null)
            {
                TempData["Error"] = "Leave request could not be found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                leaveRequest.Reject();

                _context.LeaveRequests.Update(leaveRequest);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Leave request rejected successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}