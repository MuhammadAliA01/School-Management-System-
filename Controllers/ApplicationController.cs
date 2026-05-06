using Microsoft.AspNetCore.Mvc;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;


namespace SchoolManagmentSystem.Controllers
{
    public class ApplicationController : Controller
    {


        private readonly IApplicationService _aiService; // This replaces 'docService' and 'aiService'

        // This is the Constructor - it "Injects" the service
        public ApplicationController(IApplicationService aiService)
        {
            _aiService = aiService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(DocViewModel model)
        {
            var result = await _aiService.ProcessApplicationAsync(model.IDDocument, model.ApplicationForm);

            model.ResultMessage = result;
            return View("Index", model);
        }

    }
}
