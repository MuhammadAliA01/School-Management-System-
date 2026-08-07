using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SchoolManagmentSystem.Models;
using Mscc.GenerativeAI;
using QRCoder;

namespace SchoolManagmentSystem.Controllers
{
    public class StudentProfileController : Controller
    {
       
        private readonly ApplicationDBContext _context;

        public StudentProfileController(ApplicationDBContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index(int id)
        {
        
            var studentRecord = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentId == id);

            
            if (studentRecord == null)
            {
                return NotFound("We couldn't find a student with that ID in the system.");
            }

            
            return View(studentRecord);
        }

       
        public async Task<IActionResult> StudentList(string searchString)
        {
            
            var students = from s in _context.Students
                           select s;

            
            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.FullName.Contains(searchString));
            }

            
            return View(await students.ToListAsync());
        }

        public async Task<IActionResult> ViewQR(int id)
        {
            var student = _context.Students.Where(x => x.StudentId == id).Select(x => x).FirstOrDefault();

            string payload = $"{student.StudentId}|{student.FullName}";

            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrData);
            byte[] qrBytes = qrCode.GetGraphic(10);

            ViewBag.QRCode = Convert.ToBase64String(qrBytes);

            return View(student);

        }
    }
}    

