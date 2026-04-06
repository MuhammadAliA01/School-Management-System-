using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Types;
using System.Text.Json;

namespace SchoolManagmentSystem.ServicesAi.EmploymentAI
{
    public class AIScoringService
    {
        private readonly string _apikey = "AIzaSyBMtcpMmNvDmP20FmYbXp8nfElCZSsYxRI";

        public async Task<(int Score, string Summary)> GetSuitabilityScore(string cvText, string role)
        {
            try
            {
                var googleAI = new GoogleAI(_apikey);
                var model = googleAI.GenerativeModel(Model.Gemini25Flash);

                string prompt = $@"Analyze the following CV for the position of {role}. If the role is 'Teacher', look for education degrees and classroom experience. Since it is a special needs school, those with special needs experience is more priority, particularly those with experience with autistic kids. But normal teachers can suffice. If the role is 'Cleaner' look for physical reliability and cleaning experience.


             CV Text: {cvText}


             Return ONLY a Json object with two fields: 'score' (an integer from 1-10) and 'summary' (a brief explanation) No markdown, no ```json tags.";

                var response = await model.GenerateContent(prompt);
                var jsonResponse = response.Text;

                Console.WriteLine("RAW AI RESPONSE: " + jsonResponse);

                if (jsonResponse.StartsWith("```"))
                {
                    jsonResponse = jsonResponse.Replace("```json", "").Replace("```", "").Trim();
                }
                
                var result = JsonSerializer.Deserialize<AIResult>(jsonResponse);

                return (result?.score ?? 0, result?.summary ?? "No summary provided.");
            }
            catch (Exception ex)
            {
                return (0, "AI scoring failed: " + ex.Message);
            }
        }


    }

    public class AIResult
    {
        public int score { get; set; }
        public string summary { get; set; }
    }
}
