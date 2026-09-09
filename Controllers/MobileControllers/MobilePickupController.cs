using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;

namespace SchoolManagmentSystem.Controllers.MobileControllers
{
    [ApiController]
    [Route("api/MobilePickup")]
    public class MobilePickupController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly FirebaseNotificationService _firebaseNotificationService;


        public MobilePickupController(ApplicationDBContext context, FirebaseNotificationService firebaseNotificationService)
        {
            _context = context;
            _firebaseNotificationService = firebaseNotificationService;

        }

        // TEACHER SIDE: Verify PIN
        [HttpPost("verifydelegate")]
        public async Task<IActionResult> VerifyDelegateCode(
    [FromBody] DelegateDto request)
        {
            if (string.IsNullOrWhiteSpace(request.DelegateCode))
            {
                return BadRequest(new
                {
                    message = "Delegate code is required."
                });
            }

            var pickup = await _context.StudentPickups
       .Include(p => p.Student)
       .ThenInclude(s => s.Parent)
       .FirstOrDefaultAsync(p =>
           p.DelegatePIN == request.DelegateCode &&
           p.PickupStatus == "Not Picked Up");

            if (pickup == null)
            {
                return BadRequest(new
                {
                    message = "Invalid code or student already picked up."
                });
            }

            pickup.PickupStatus = "Picked Up";
            pickup.PickupTime = DateTime.Now;

            await _context.SaveChangesAsync();

            if (pickup.Student.Parent != null &&
       !string.IsNullOrWhiteSpace(pickup.Student.Parent.FCMToken))
            {
                await _firebaseNotificationService.SendNotification(
                    pickup.Student.Parent.FCMToken,
                    "Picked Up",
                    $"{pickup.Student.FullName} has been picked up"
                );
            }

            return Ok(new
            {
                message = "Delegate Code Verified! Student released.",
                studentId = pickup.StudentId,
                studentName = pickup.Student.FullName,
                pickupTime = pickup.PickupTime
            });
        }

        // PARENT SIDE: Confirm via QR
        [HttpPost("confirmpickup")]
        public async Task<IActionResult> ConfirmParentPickup(
    [FromBody] QrScanDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new
                    {
                        message = "Request is empty."
                    });
                }

                if (request.StudentId <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Invalid student ID."
                    });
                }

                if (string.IsNullOrWhiteSpace(request.ScannedData))
                {
                    return BadRequest(new
                    {
                        message = "QR code data is missing."
                    });
                }

                // Make sure it is our pickup QR
                if (!request.ScannedData.StartsWith("PICKUP_AUTH_"))
                {
                    return BadRequest(new
                    {
                        message = "Invalid QR Code."
                    });
                }

                // Find the EXISTING student
                var student = await _context.Students
    .Include(s => s.Parent)
    .FirstOrDefaultAsync(s => s.StudentId == request.StudentId);

                if (student == null)
                {
                    return NotFound(new
                    {
                        message = "Student not found."
                    });
                }

                // Check if this student already has an active pickup record
                var pickup = await _context.StudentPickups
                    .FirstOrDefaultAsync(p =>
                        p.StudentId == request.StudentId &&
                        p.PickupStatus == "Not Picked Up");

                if (pickup == null)
                {
                    // Create a new pickup record
                    pickup = new StudentPickup
                    {
                        StudentId = request.StudentId,
                        PickupStatus = "Picked Up",
                        PickupTime = DateTime.Now,
                        CreatedDate = DateTime.Now
                    };

                    _context.StudentPickups.Add(pickup);
                }
                else
                {
                    // Existing pickup record
                    pickup.PickupStatus = "Picked Up";
                    pickup.PickupTime = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                if (student.Parent != null && !string.IsNullOrEmpty(student.Parent.FCMToken))
                {
                    await _firebaseNotificationService.SendNotification(
                        student.Parent.FCMToken,
                        "Picked Up",
                        $"{student.FullName} has been picked up"
                    );
                }

                return Ok(new
                {
                    message = "Collection successful!",
                    studentId = student.StudentId,
                    studentName = student.FullName,
                    pickupTime = pickup.PickupTime
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while processing pickup.",
                    error = ex.Message
                });
            }
        }

        [HttpPost("createdelegate")]
        public async Task<IActionResult> CreateDelegateCode(
    [FromBody] CreateDelegateDto request)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentId == request.StudentId);

            if (student == null)
            {
                return NotFound(new
                {
                    message = "Student not found."
                });
            }

            var pickup = await _context.StudentPickups
                .FirstOrDefaultAsync(p =>
                    p.StudentId == request.StudentId &&
                    p.PickupStatus == "Not Picked Up");

            if (pickup == null)
            {
                pickup = new StudentPickup
                {
                    StudentId = request.StudentId,
                    DelegatePIN = request.DelegateCode,
                    PickupStatus = "Not Picked Up"
                };

                _context.StudentPickups.Add(pickup);
            }
            else
            {
                pickup.DelegatePIN = request.DelegateCode;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Delegate code created successfully."
            });
        }
    }

    public class DelegateDto { public string? DelegateCode { get; set; } }
    public class QrScanDto { public int StudentId { get; set; } public string? ScannedData { get; set; } }
}
