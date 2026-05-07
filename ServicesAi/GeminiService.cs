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
            // api keys
            string apiKey = "AIzaSyBkyYXI2R303zAbWVzrQ8frurKAoM70gv4";
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            // pdf
            

            // 👉 4. Notice how we injected the "medicalText" right into the middle of the prompt!
            string prompt = $"I have a student with {level} and {type} autism. The parent noted: '{parentComments}'. Here is their medical evaluation: '{medicalText}'. Based on this exact profile, give me exactly 3 short teaching methods.";

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

            return jsonNode["candidates"][0]["content"]["parts"][0]["text"].ToString();
        }
    }
}
