using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        public IActionResult Dashboard()
        {
            var allStudents = Context.StudentRegistrations.ToList();
            return View(allStudents); 
        }

        [Authorize]
        public async Task<IActionResult> GenerateTeachingMethods(int id)
        {
            var student = Context.StudentRegistrations.FirstOrDefault(s => s.Id == id);

            if (student == null) return Content("Error: Student not found!");

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

            ViewBag.StudentName = student.ChildName;
            ViewBag.AiAdvice = answer;
            return View();
        }
    }
}
