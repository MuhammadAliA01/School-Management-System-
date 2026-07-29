using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mscc.GenerativeAI;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.ServicesAi.EmploymentAI;
using Stripe;
using System.Text.RegularExpressions;
using UglyToad.PdfPig; 

namespace SchoolManagmentSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly string _apiKey;
        private readonly IWebHostEnvironment _environment;
        private readonly CVService _cvService;


        public EmployeeController(ApplicationDBContext context, IConfiguration config, IWebHostEnvironment environment, CVService cvService)
        {
            _context = context;
            _apiKey = config["ApiSettings:ApiKey"] ?? "";
            _environment = environment;
            _cvService = cvService;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var employees = _context.Employees.OrderByDescending(e => e.Id).AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
                employees = employees.Where(s => s.FullName.Contains(searchString));
            return View(await employees.ToListAsync());
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee, IFormFile? contractFile)
        {
            ModelState.Remove("DocumentPath");
            if (contractFile != null && contractFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(contractFile.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
             
                using (var stream = new FileStream(path, FileMode.Create)) { await contractFile.CopyToAsync(stream); }

                employee.DocumentPath = "/uploads/" + fileName;

            }

            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                var cvFile = Path.GetFileName(employee.DocumentPath);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", cvFile);
                var cvService = new CVService();
                var cvText = await cvService.ExtractTextFromPDFAsync(contractFile);
                var aiFeedback = await UploadWithAI(cvText, contractFile);
                

                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> UploadWithAI(string cvText, IFormFile file)
        {
            if (file == null || file.Length == 0) return RedirectToAction(nameof(Index));

            
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create)) { await file.CopyToAsync(stream); }

            string extractedText = await _cvService.ExtractTextFromPDFAsync(filePath);



            //string extractedText = cvText;
           /* try
            {
                using (var pdf = PdfDocument.Open(filePath))
                {
                    foreach (var page in pdf.GetPages())
                    {
                        extractedText += page.Text + " ";
                    }
                }
            }
            catch { extractedText = "Text extraction failed."; }*/

            // 3. Defaults
            string finalName = "AI Scanned (" + DateTime.Now.ToString("HH:mm") + ")";
            string finalPos = "Staff";

            // 4. Send the VARIABLE to Gemini
            try
            {
                var googleAI = new GoogleAI(_apiKey);
                var model = googleAI.GenerativeModel("gemini-2.5-flash");

                string prompt = $"Read this contract text: '{extractedText}'. " +
                                "Find the Full Name and Position. " +
                                "Return ONLY JSON: {\"FullName\":\"\", \"Position\":\"\"}";

                var response = await model.GenerateContent(prompt);
                string rawJson = response.Text;

                try
                {
                    // Clean potential markdown if the AI ignores the prompt instruction
                    if (rawJson.Contains("```"))
                    {
                        rawJson = rawJson.Replace("```json", "").Replace("```", "").Trim();
                    }

                    var aiData = System.Text.Json.JsonSerializer.Deserialize<Employee>(rawJson);

                    if (aiData != null)
                    {
                        finalName = aiData.FullName ?? finalName;
                        finalPos = aiData.Position ?? finalPos;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("JSON Parsing failed: " + ex.Message);
                }

                Console.WriteLine(response + " AAAAAAAAAAAAAAAAAAAAAaaaaa");

                ViewBag.EmpInfo = response;

                //var match = Regex.Match(response.Text ?? "", @"\{.*\}", RegexOptions.Singleline);

                /*if (match.Success)
                {
                    var aiData = System.Text.Json.JsonSerializer.Deserialize<Employee>(match.Value);
                    if (!string.IsNullOrEmpty(aiData?.FullName)) finalName = aiData.FullName;
                    if (!string.IsNullOrEmpty(aiData?.Position)) finalPos = aiData.Position;
                }*/
            }
            catch { }

            var newEmployee = new Employee
            {
                FullName = finalName,
                Position = finalPos,
                DateJoined = DateTime.Now,
                DocumentPath = "/uploads/" + fileName
            };

            _context.Employees.Add(newEmployee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}