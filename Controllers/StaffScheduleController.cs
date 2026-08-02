using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers
{
    public class StaffScheduleController : Controller
    {
        // FIXED: Capitalized 'DB' to match your ApplicationDBContext.cs file
        private readonly ApplicationDBContext _context;

        public StaffScheduleController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Assign()
        {
            // 1. Set the automated date (e.g., scheduling for tomorrow)
            DateTime targetDate = DateTime.Today.AddDays(1);

            // 2. Find all classes that ALREADY have a schedule for tomorrow
            var assignedClassIds = _context.StaffSchedules
                .Where(s => s.ScheduleDate.Date == targetDate.Date)
                .Select(s => s.ClassId)
                .ToList();

            // 3. Get all classes that are currently UNASSIGNED for tomorrow
            var unassignedClasses = _context.Classes
                .Where(c => !assignedClassIds.Contains(c.ClassId)) // Assuming the Primary Key is ClassId
                .ToList();

            // 4. Get all active staff members
            var allStaff = _context.Employees.ToList();

            // 5. Build the dashboard view model
            var viewModel = new AssignScheduleViewModel
            {
                AvailableStaff = allStaff,
                UnassignedClasses = unassignedClasses,
                TargetScheduleDate = targetDate
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RunAutoAssign()
        {
            DateTime targetDate = DateTime.Today.AddDays(1);
            var staff = _context.Employees.ToList();

            // Find unassigned classes again to ensure data is fresh
            var assignedClassIds = _context.StaffSchedules
                .Where(s => s.ScheduleDate.Date == targetDate.Date)
                .Select(s => s.ClassId)
                .ToList();

            var unassignedClasses = _context.Classes
                .Where(c => !assignedClassIds.Contains(c.ClassId))
                .ToList();

            // Safety checks
            if (!unassignedClasses.Any())
            {
                TempData["SuccessMessage"] = "All classes are already assigned for this shift! No action needed.";
                return RedirectToAction(nameof(Assign));
            }
            if (!staff.Any())
            {
                ModelState.AddModelError("", "No staff members found in the database.");
                return RedirectToAction(nameof(Assign));
            }

            // THE ROUND-ROBIN ALGORITHM
            int staffIndex = 0;
            int staffCount = staff.Count;
            int assignedCount = 0;

            foreach (var emptyClass in unassignedClasses)
            {
                // Select the staff member based on the current index
                var selectedStaff = staff[staffIndex % staffCount];

                // Create the new schedule automatically
                var newSchedule = new StaffSchedule
                {
                    TeacherId = selectedStaff.Id, // Replaced EmployeeId with TeacherId
                    ClassId = emptyClass.ClassId, // (Assuming ClassId is correct for the Class model!)         
                    ScheduleDate = targetDate
                };

                _context.StaffSchedules.Add(newSchedule);

                // Move to the next staff member for the next iteration
                staffIndex++;
                assignedCount++;
            }

            // Save all the new assignments to the database at once
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Automated Scheduling Complete: Successfully assigned {assignedCount} shifts!";
            return RedirectToAction(nameof(Assign));
        }
    }
    }
