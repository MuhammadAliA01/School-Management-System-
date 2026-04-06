using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;


namespace SchoolManagmentSystem.ServicesAi
{
    public class GeminiService : aiRulebook
    {

        public async Task<string> GetAdvice(string level, string type, string parentComments, string medicalText)
        {
            string apiKey = "AIzaSyBkyYXI2R303zAbWVzrQ8frurKAoM70gv4";
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            string prompt = $"I have a student with {level} and {type} autism. The parent noted: '{parentComments}'. Here is their medical evaluation: '{medicalText}'. Based on this exact profile, give me exactly 3 short teaching methods.";

            var package = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
            var content = new StringContent(JsonSerializer.Serialize(package), Encoding.UTF8, "application/json");

            var client = new HttpClient();
            var response = await client.PostAsync(url, content);

            string rawJson = await response.Content.ReadAsStringAsync();
            JsonNode jsonNode = JsonNode.Parse(rawJson);

            if (!response.IsSuccessStatusCode)
            {
                return $" GEMINI API ERROR: {rawJson}";
            }

            return jsonNode["candidates"][0]["content"]["parts"][0]["text"].ToString();
        }
    }
}
