using UglyToad.PdfPig;

namespace SchoolManagmentSystem.ServicesAi.EmploymentAI
{
    public class CVService
    {
        public async Task<string> ExtractTextFromPDFAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return string.Empty;

            return await Task.Run(() =>
            {
                try
                {

                    using var stream = file.OpenReadStream();
                    using var pdf = PdfDocument.Open(stream);
                    var text = "";

                    foreach (var page in pdf.GetPages())
                    {
                        text += page.Text + " ";
                    }

                    return text;
                }
                catch
                {
                    return "Error extracting text from PDF.";
                }
            });
        }

        public async Task<string> ExtractTextFromPDFAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return string.Empty;

            return await Task.Run(() =>
            {
                try
                {
                    using var stream = File.OpenRead(filePath);
                    using var pdf = PdfDocument.Open(stream);

                    var text = "";

                    foreach (var page in pdf.GetPages())
                    {
                        text += page.Text + " ";
                    }

                    return text;
                }
                catch
                {
                    return "Error extracting text from PDF.";
                }
            });
        }

    }
}
