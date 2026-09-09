using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;

namespace SchoolManagmentSystem.Controllers
{
    public class StaffScheduleController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly FirebaseNotificationService _firebaseNotificationService;

        public StaffScheduleController(
            ApplicationDBContext context,
            FirebaseNotificationService firebaseNotificationService)
        {
            _context = context;
            _firebaseNotificationService = firebaseNotificationService;
        }


        // ==========================================
        // ASSIGNMENT DASHBOARD
        // ==========================================

        [HttpGet]
        public IActionResult Assign()
        {
            // Schedule for tomorrow
            DateTime targetDate = DateTime.Today.AddDays(1);


            // ==========================================
            // GET EXISTING SCHEDULES
            // ==========================================

            var schedules = _context.StaffSchedules
                .Include(s => s.Class)
                .Where(s => s.ScheduleDate.Date == targetDate.Date)
                .ToList();


            // ==========================================
            // GET CLASSES THAT ARE ALREADY ASSIGNED
            // ==========================================

            var assignedClassIds = schedules
                .Select(s => s.ClassId)
                .ToList();


            // ==========================================
            // GET UNASSIGNED CLASSES
            // ==========================================

            var unassignedClasses = _context.Classes
                .Where(c => !assignedClassIds.Contains(c.ClassId))
                .ToList();


            // ==========================================
            // GET AVAILABLE STAFF
            // ==========================================

            var allStaff = _context.Staffs
                .ToList();


            // ==========================================
            // BUILD VIEW MODEL
            // ==========================================

            var viewModel = new AssignScheduleViewModel
            {
                AvailableStaff = allStaff,

                UnassignedClasses = unassignedClasses,

                Schedules = schedules,

                TargetScheduleDate = targetDate
            };


            return View(viewModel);
        }


        // ==========================================
        // AUTOMATIC ASSIGNMENT
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunAutoAssign()
        {
            // Schedule for tomorrow
            DateTime targetDate = DateTime.Today.AddDays(1);


            // ==========================================
            // GET ALL STAFF
            // ==========================================

            var staff = await _context.Staffs
                .ToListAsync();


            // ==========================================
            // GET EXISTING SCHEDULES FOR TOMORROW
            // ==========================================

            var existingSchedules = await _context.StaffSchedules
                .Where(s => s.ScheduleDate.Date == targetDate.Date)
                .ToListAsync();


            // ==========================================
            // FIND CLASSES ALREADY ASSIGNED
            // ==========================================

            var assignedClassIds = existingSchedules
                .Select(s => s.ClassId)
                .ToList();


            // ==========================================
            // FIND UNASSIGNED CLASSES
            // ==========================================

            var unassignedClasses = await _context.Classes
                .Where(c => !assignedClassIds.Contains(c.ClassId))
                .ToListAsync();


            // ==========================================
            // SAFETY CHECK - NO CLASSES
            // ==========================================

            if (!unassignedClasses.Any())
            {
                TempData["SuccessMessage"] =
                    "All classes are already assigned for this date!";

                return RedirectToAction(nameof(Assign));
            }


            // ==========================================
            // SAFETY CHECK - NO STAFF
            // ==========================================

            if (!staff.Any())
            {
                TempData["SuccessMessage"] =
                    "No staff members were found in the database.";

                return RedirectToAction(nameof(Assign));
            }


            // ==========================================
            // FIND TEACHERS ALREADY ASSIGNED TOMORROW
            // ==========================================

            var alreadyAssignedTeacherIds = existingSchedules
                .Select(s => s.TeacherId)
                .ToHashSet();


            // ==========================================
            // GET ONLY AVAILABLE TEACHERS
            //
            // A teacher who already has a class tomorrow
            // cannot be assigned another class.
            // ==========================================

            var availableStaff = staff
                .Where(s => !alreadyAssignedTeacherIds.Contains(s.id))
                .ToList();


            // ==========================================
            // AUTOMATIC ASSIGNMENT
            //
            // Each teacher can only be assigned
            // ONE class for the day.
            // ==========================================

            int assignedCount = 0;


            // Keep track of staff that were assigned
            // so that we can notify them after saving.
            var assignedStaff = new List<(dynamic Staff, string ClassName)>();


            foreach (var emptyClass in unassignedClasses)
            {
                // If there are no teachers left,
                // leave the remaining classes unassigned.
                if (!availableStaff.Any())
                {
                    break;
                }


                // ==========================================
                // PICK NEXT AVAILABLE TEACHER
                // ==========================================

                var selectedStaff = availableStaff[0];


                // ==========================================
                // CREATE NEW SCHEDULE
                // ==========================================

                var newSchedule = new StaffSchedule
                {
                    TeacherId = selectedStaff.id,

                    ClassId = emptyClass.ClassId,

                    ScheduleDate = targetDate,

                    ShiftType = "Full Day"
                };


                _context.StaffSchedules.Add(newSchedule);


                // ==========================================
                // KEEP TRACK OF ASSIGNMENT
                // FOR NOTIFICATION
                // ==========================================

                assignedStaff.Add(
                    (selectedStaff, emptyClass.ClassName)
                );


                // ==========================================
                // REMOVE TEACHER FROM AVAILABLE LIST
                //
                // This prevents the same teacher from
                // being assigned another class.
                // ==========================================

                availableStaff.RemoveAt(0);


                assignedCount++;
            }


            // ==========================================
            // SAVE TO DATABASE
            // ==========================================

            await _context.SaveChangesAsync();


            // ==========================================
            // SEND NOTIFICATIONS TO ASSIGNED STAFF
            // ==========================================

            foreach (var assignment in assignedStaff)
            {
                var selectedStaff = assignment.Staff;
                var className = assignment.ClassName;


                // Staff must have an FCM token
                if (!string.IsNullOrEmpty(selectedStaff.FCMToken))
                {
                    bool success =
                        await _firebaseNotificationService.SendNotification(
                            selectedStaff.FCMToken,

                            "New Class Assignment",

                            $"You have been assigned to {className} " +
                            $"on {targetDate:dd/MM/yyyy}."
                        );


                    // ==========================================
                    // INVALID / EXPIRED FCM TOKEN
                    // ==========================================

                    if (!success)
                    {
                        selectedStaff.FCMToken = null;
                    }
                }
            }


            // Save any cleared FCM tokens
            await _context.SaveChangesAsync();


            // ==========================================
            // CALCULATE REMAINING CLASSES
            // ==========================================

            int remainingClasses =
                unassignedClasses.Count - assignedCount;


            // ==========================================
            // SUCCESS MESSAGE
            // ==========================================

            if (remainingClasses > 0)
            {
                TempData["SuccessMessage"] =
                    $"Automated Scheduling Complete! " +
                    $"{assignedCount} classes were assigned. " +
                    $"{remainingClasses} classes remain unassigned " +
                    $"because there were not enough available teachers.";
            }
            else
            {
                TempData["SuccessMessage"] =
                    $"Automated Scheduling Complete! " +
                    $"All {assignedCount} classes were successfully assigned.";
            }


            // ==========================================
            // RETURN TO DASHBOARD
            // ==========================================

            return RedirectToAction(nameof(Assign));
        }


        // ==========================================
        // FLUTTER - GET STAFF SCHEDULE
        // ==========================================

        [HttpGet]
        [Route("api/StaffSchedule/staff/{staffId}")]
        public async Task<IActionResult> GetStaffSchedule(int staffId)
        {
            var schedules = await _context.StaffSchedules
                .Where(s => s.TeacherId == staffId)
                .OrderBy(s => s.ScheduleDate)
                .Select(s => new
                {
                    scheduleId = s.ScheduleId,

                    teacherId = s.TeacherId,

                    classId = s.ClassId,

                    className = _context.Classes
                        .Where(c => c.ClassId == s.ClassId)
                        .Select(c => c.ClassName)
                        .FirstOrDefault(),

                    scheduleDate = s.ScheduleDate,

                    shiftType = s.ShiftType
                })
                .ToListAsync();


            return Ok(schedules);
        }
    }
}