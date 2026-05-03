using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Types;
using SchoolManagmentSystem.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SchoolManagmentSystem.ServicesAi.IncidentAI
{
    public class IncidentAnalysisService
    {
        
        private readonly string _apikey = "";

        public IncidentAnalysisService(IConfiguration configuration)
        {
            _apikey = configuration["ApiSettings:ApiKey"];
        }

        public async Task<IncidentAnalysisResult> AnalyzeSingleIncidentAsync(Incident incident)
        {
            try
            {
                var googleAI = new GoogleAI(_apikey);
                var model = googleAI.GenerativeModel(Model.Gemini25Flash);

                // We feed the AI the specific details of this one row
                string prompt = $@"
                    Analyze this school incident record for a special needs student.
                    
                    DETAILS:
                    - Student: {incident.StudentName}
                    - Description: {incident.Description}
                    - Action Taken: {incident.ActionTaken}

                    TASK:
                    1. Evaluate the severity from 1 to 5.
                    2. Briefly evaluate if the 'Action Taken' was appropriate for a special needs setting.
                    3. Provide one professional 'Next Step' for the teacher.

                    RETURN ONLY RAW JSON. No markdown.
                    Format: {{ 
                        ""impact"": ""string"", 
                        ""actionEvaluation"": ""string"", 
                        ""professionalAdvice"": ""string"", 
                        ""severityLevel"": integer 
                    }}";

                var response = await model.GenerateContent(prompt);
                string rawText = response.Text ?? "";

                // Regex to extract the JSON block safely
                var match = Regex.Match(rawText, @"\{.*\}", RegexOptions.Singleline);
                if (!match.Success) return new IncidentAnalysisResult { Impact = "Analysis failed." };

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<IncidentAnalysisResult>(match.Value, options);
            }
            catch (Exception ex)
            {
                return new IncidentAnalysisResult { Impact = "Error: " + ex.Message };
            }
        }
    }
}
