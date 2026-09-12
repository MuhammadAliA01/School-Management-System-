using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers
{
    public class PayrollController : Controller
    {

        private readonly ApplicationDBContext _context;

        public PayrollController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> PayrollDashboard()
        {

            var empList = await _context.Staffs.ToListAsync();


            foreach (var id in empList)
            {
                var empID = id.id;
                var salary = 0.0;

                var hoursWorked = _context.StaffAttendances.Where(x => x.StaffId == id.id).Select(x => x.TotalHoursWorked).FirstOrDefault();

                if (id.Role == "Admin")
                {
                    salary = 30000;
                }
                else if (id.Role == "Teacher")
                {
                    salary = 40000;

                }
                else if (id.Role == "Principal")
                {
                    salary = 50000;

                }
                else if (id.Role == "Cleaner")
                {
                    salary = 40 * Convert.ToDouble(hoursWorked);

                }

                var exists = _context.EmployeeEarnings.Any(x => x.EmployeeID == id.id && x.payCycle.Month == DateTime.Now.Month && x.payCycle.Year == DateTime.Now.Year);

                if (!exists)
                {
                    var newEntry = new EmployeeEarnings
                    {
                        EmployeeID = empID,
                        salaryAmt = salary,
                        payCycle = DateTime.Now


                    };

                    _context.EmployeeEarnings.Add(newEntry);

                }


            }

            _context.SaveChanges();

            var listOfEarnings = await _context.EmployeeEarnings.ToListAsync();
    
            return View(listOfEarnings);
        }
    }
}
