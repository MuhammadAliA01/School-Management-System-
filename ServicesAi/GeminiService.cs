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
            // API setup
            string apiKey = ""; // Keep your key secure!
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

            // 1. Create the prompt
            string promptText = $"I have a student with {level} and {type} autism. The parent noted: '{parentComments}'. " +
                                $"Here is their medical evaluation: '{medicalText}'. Based on this profile, give me exactly 3 short teaching methods.";

            // 2. FIX: Define the 'package' object for the Gemini API
            var package = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = promptText } } }
                }
            };

            // 3. FIX: Simplified StringContent call to avoid MediaTypeHeaderValue errors
            var jsonString = JsonSerializer.Serialize(package);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            try
            {
                var response = await client.PostAsync(url, content);
                string rawJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonNode = JsonNode.Parse(rawJson);
                    var aiText = jsonNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                    return !string.IsNullOrEmpty(aiText) ? aiText.Trim() : "AI Error: Response empty.";
                }

                return $"API Error: {response.StatusCode}";
            }
            catch (Exception ex)
            {
                // 4. FIX: Ensure all code paths return a value
                return $"Request Exception: {ex.Message}";
            }

            
        
            
        }
        



    public async Task<string> GetConductPlan(string description, string severity)
        {
            // API setup
            string apiKey = ""; // Keep your key secure!
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            // 1. Create the prompt
            string promptText = $@"
You are generating a behavioural intervention response for a school management system.

Analyze the incident and generate a clear intervention plan suitable for a special needs school environment.

RULES:
- Keep the response professional, supportive, and concise.
- Do NOT mention being an AI, assistant, therapist, specialist, or any role/persona.
- Do NOT include introductions or disclaimers.
- Use plain text only.
- Do NOT use markdown symbols such as **, ##, or bullet symbols.
- Use clean spacing and line breaks exactly as shown below.
- Keep the response easy for teachers and administrators to read.


OUTPUT FORMAT:

Incident Analysis:
[Write 2-4 sentences here]

Recommended Actions:
1. [Action]
2. [Action]
3. [Action]

Follow-Up Plan:
Short-term:
- [Short-term action]
- [Short-term action]

Long-term:
- [Long-term action]
- [Long-term action]

INCIDENT DESCRIPTION:
{description}

INCIDENT SEVERITY:
{severity}
";

            // 2. FIX: Define the 'package' object for the Gemini API
            var package = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = promptText } } }
                }
            };

            // 3. FIX: Simplified StringContent call to avoid MediaTypeHeaderValue errors
            var jsonString = JsonSerializer.Serialize(package);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            try
            {
                var response = await client.PostAsync(url, content);
                string rawJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonNode = JsonNode.Parse(rawJson);
                    var aiText = jsonNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                    return !string.IsNullOrEmpty(aiText) ? aiText.Trim() : "AI Error: Response empty.";
                }

                return $"API Error: {response.StatusCode}";
            }
            catch (Exception ex)
            {
                // 4. FIX: Ensure all code paths return a value
                return $"Request Exception: {ex.Message}";
            }




        }
    }


}
