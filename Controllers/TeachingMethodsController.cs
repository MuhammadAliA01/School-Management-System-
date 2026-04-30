using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers
{
    public class TeachingMethodsController : Controller
    {
        private readonly ApplicationDBContext Context;
        private readonly IWebHostEnvironment _env; 

        public TeachingMethodsController(ApplicationDBContext context, IWebHostEnvironment env)
        {
            
            Context = context;
            _env = env;
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            var allStudents = Context.Students.ToList();
            return View(allStudents); 
        }

        [Authorize]
        public async Task<IActionResult> GenerateTeachingMethods(int id)
        {
            var student = Context.Students.FirstOrDefault(s => s.StudentId == id);

            if (student == null) return Content("Error: Student not found!");

            if (!string.IsNullOrEmpty(student.AiTeachingPlan))
            {
                // YES! Skip the AI entirely, use the saved plan, and load the page instantly!
                ViewBag.StudentName = student.FullName;
                ViewBag.AiAdvice = student.AiTeachingPlan;
                return View();
            }

            string medicalText = "";
            string folderPath = _env.WebRootPath;

            // 2. Loop through every document linked to this student
            if (student.Documents != null && student.Documents.Any())
            {
                foreach (var doc in student.Documents)
                {
                    // Combine the web root with the saved relative path
                    string physicalPath = Path.Combine(folderPath, doc.FilePath.TrimStart('/'));

                    if (System.IO.File.Exists(physicalPath))
                    {
                        using (UglyToad.PdfPig.PdfDocument pdf = UglyToad.PdfPig.PdfDocument.Open(physicalPath))
                        {
                            foreach (UglyToad.PdfPig.Content.Page page in pdf.GetPages())
                            {
                                medicalText += page.Text + " ";
                            }
                        }
                    }
                }
            }

            // 3. Send the combined text from ALL PDFs to the AI
            var consensusEngine = new SchoolManagmentSystem.ServicesAi.DecisionMaker();

            string answer = await consensusEngine.FindBestMethods(
                student.AutismLevel,
                student.AutismType,
                student.Comments,
                medicalText // This now contains text from all uploaded files
            );

            student.AiTeachingPlan = answer;
            Context.SaveChanges();

            ViewBag.StudentName = student.FullName;
            ViewBag.AiAdvice = answer;
            return View();

            /*string medicalText = "";
            string folderPath = _env.WebRootPath;

            if (student.Documents != null && student.Documents.Any())
            {
                foreach (var doc in student.Documents)
                {
                    // Combine the web root with the saved relative path
                    string physicalPath = Path.Combine(folderPath, doc.FilePath.TrimStart('/'));

                    if (System.IO.File.Exists(physicalPath))
                    {
                        using (UglyToad.PdfPig.PdfDocument pdf = UglyToad.PdfPig.PdfDocument.Open(physicalPath))
                        {
                            foreach (UglyToad.PdfPig.Content.Page page in pdf.GetPages())
                            {
                                medicalText += page.Text + " ";
                            }
                        }
                    }
                }
            }

            var consensusEngine = new SchoolManagmentSystem.ServicesAi.DecisionMaker();

            
            using (UglyToad.PdfPig.PdfDocument document = UglyToad.PdfPig.PdfDocument.Open(physicalPath))
            {
                foreach (UglyToad.PdfPig.Content.Page page in document.GetPages())
                {
                    medicalText += page.Text + " ";
                }
            }
            string answer = await consensusEngine.FindBestMethods(
                student.AutismLevel,
                student.AutismType,
                student.Comments,
                medicalText
            );

            student.AiTeachingPlan = answer;
            Context.SaveChanges();
            ViewBag.StudentName = student.FullName;
            ViewBag.AiAdvice = answer;
            return View();*/
        }
    }
}
