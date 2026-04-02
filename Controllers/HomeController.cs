using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;
using System.Diagnostics;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;
using SchoolManagmentSystem.Services;
using System.IO;
using System.Drawing;

namespace SchoolManagmentSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            
        }

        public IActionResult HomePage()
        {

            
            return View();

            
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult Gallery()
        {
            return View();
        }

        public IActionResult Calendar()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

     
        public IActionResult RegistrationService()
        {
            return View();
        }

        public IActionResult DownloadPdf()
        {
            if (PdfSharp.Fonts.GlobalFontSettings.FontResolver == null)
                PdfSharp.Fonts.GlobalFontSettings.FontResolver = new MyFontResolver();

            var registrationService = new RegistrationService();
            var document = registrationService.GetRegistration();

            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream);
                byte[] bytes = stream.ToArray();

                // This returns the file directly to the browser
                return File(bytes, "application/pdf", "Registration.pdf");
            }
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
public class MyFontResolver : IFontResolver
{
    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
        {
            if (isBold && isItalic) return new FontResolverInfo("arialbi.ttf");
            if (isBold) return new FontResolverInfo("arialbd.ttf");
            if (isItalic) return new FontResolverInfo("ariali.ttf");
            return new FontResolverInfo("arial.ttf");
        }

        if (familyName.Equals("Courier New", StringComparison.OrdinalIgnoreCase))
        {
            if (isBold && isItalic) return new FontResolverInfo("courbi.ttf");
            if (isBold) return new FontResolverInfo("courbd.ttf");
            if (isItalic) return new FontResolverInfo("couri.ttf");
            return new FontResolverInfo("cour.ttf");
        }

        if (familyName.Equals("Times New Roman", StringComparison.OrdinalIgnoreCase))
        {
            if (isBold && isItalic) return new FontResolverInfo("timesbi.ttf");
            if (isBold) return new FontResolverInfo("timesbd.ttf");
            if (isItalic) return new FontResolverInfo("timesi.ttf");
            return new FontResolverInfo("times.ttf");
        }

        return new FontResolverInfo("arial.ttf");
    }

    public byte[] GetFont(string faceName)
    {
        string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), faceName);

        if (File.Exists(fontPath))
        {
            return File.ReadAllBytes(fontPath);
        }

        return File.ReadAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf"));
    }
}




