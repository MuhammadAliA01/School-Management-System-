using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;
using System.Net;
using System.Net.Mail;


namespace SchoolManagmentSystem.Controllers
{
    public class StudentApplicationsController : Controller
    {

        private readonly ApplicationDBContext _context;

        public StudentApplicationsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> StudentList()
        {
            return View(await _context.StudentRegistrations.ToListAsync());
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var student = await _context.StudentRegistrations.FindAsync(id);
            if (student != null)
            {
                student.Status = newStatus;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("StudentList");
        }



    }
}
