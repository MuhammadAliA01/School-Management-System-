using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers
{
    public class TeachingMethodsController : Controller
    {
        private readonly ApplicationDBContext Context;

        public TeachingMethodsController(ApplicationDBContext context)
        {
            Context = context;
        }

        public IActionResult Dashboard()
        {
            var allStudents = Context.Students.ToList();
            return View(allStudents); 
        }

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

            var consensusEngine = new SchoolManagmentSystem.ServicesAi.DecisionMaker();

            string medicalText = "";
            using (UglyToad.PdfPig.PdfDocument document = UglyToad.PdfPig.PdfDocument.Open(student.PdfPath))
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
            return View();
        }
    }
}
