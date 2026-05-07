using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SchoolManagmentSystem.ServicesAi
{
    public class GeminiService
    {
        public async Task<string> GetAdvice(string level, string type, string parentComments, string medicalText)
        {
            // Note: Use the same verified URL format as below
            string apiKey = "AIzaSyBkyYXI2R303zAbWVzrQ8frurKAoM70gv4";
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

            string prompt = $"I have a student with {level} and {type} autism. The parent noted: '{parentComments}'. Here is their medical evaluation: '{medicalText}'. Based on this exact profile, give me exactly 3 short teaching methods.";

            return await CallGemini(url, prompt);
        }

        public async Task<string> GetConductPlan(string description, string severity)
        {
            string apiKey = "AIzaSyCh1TFL5NCop0cRAixhRa4Cz0o1b7OxnaA";
            // FIXED: Ensuring the URL follows the exact Google API pattern
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            string prompt = $@"
                You are a Senior Educational Psychologist. 
                TASK: Write a detailed 'Home Support & Precautionary Plan' for a student named Azraa.
                INCIDENT: {description}
                SEVERITY: {severity}

                REQUIREMENTS:
                - Start with a professional, empathetic opening to the parent.
                - Provide 3-4 specific PRECAUTIONARY MEASURES (e.g., medical check-ups, routine adjustments).
                - Use professional academic language and write at least 5 substantial sentences.
                - Do not use bullet points or markdown headers.";

            return await CallGemini(url, prompt);
        }

        private async Task<string> CallGemini(string url, string prompt)
        {
            var package = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(package), Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            try
            {
                var response = await client.PostAsync(url, content);
                string rawJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonNode = JsonNode.Parse(rawJson);
                    // Standard path for Gemini 1.5 Flash
                    var aiText = jsonNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                    if (!string.IsNullOrEmpty(aiText))
                    {
                        return aiText.Trim();
                    }

                    return "AI Error: Response received but text was blocked or empty.";
                }

                // This will tell us if it's a 404 (URL error) or 403 (Key error)
                return $"API Error: {response.StatusCode}. Please verify the Model URL and API Key.";
            }
            catch (Exception ex)
            {
                return $"Connection Error: {ex.Message}";
            }
        }
    }
}
