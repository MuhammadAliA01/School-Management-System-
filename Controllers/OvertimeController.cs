using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OvertimeController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public OvertimeController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost("Submit")]
        public async Task<IActionResult> SubmitOvertime([FromBody] OvertimeRequest request)
        {
            if (request == null || request.StaffId <= 0)
                return BadRequest(new { message = "Invalid overtime request data." });

            // Cannot work overtime on consecutive days.
            DateTime previousDay = request.DateOfOvertime.Date.AddDays(-1);

            bool workedYesterday = await _context.OvertimeRequests
                .AnyAsync(o => o.StaffId == request.StaffId
                            && o.DateOfOvertime.Date == previousDay
                            && o.Status != "Rejected");

            if (workedYesterday)
            {
                return BadRequest(new
                {
                    message = "Overtime blocked: You cannot work overtime on consecutive days."
                });
            }

            request.Status = "Pending";
            request.DateSubmitted = DateTime.Now;

            _context.OvertimeRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Overtime request submitted successfully."
            });
        }
    }
}