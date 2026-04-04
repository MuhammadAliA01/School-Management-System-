using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SchoolManagmentSystem.ServicesAi
{
    public class DecisionMaker
    {
        public async Task<string> FindBestMethods(string level, string type, string parentComments, string medicalText)
        {
            var gemini = new GeminiService();
            var groq = new GroqService();

            var geminiTask = gemini.GetAdvice(level, type, parentComments, medicalText);
            var groqTask = groq.GetAdvice(level, type, parentComments, medicalText);
            
            await Task.WhenAll(geminiTask, groqTask);

            string advice1 = geminiTask.Result;
            string advice2 = groqTask.Result;

           
            string masterPrompt = $@"
            You are a Senior Special Education Coordinator. 
            I asked two different experts for teaching methods for a student.
            Expert 1 said: '{advice1}'
            Expert 2 said: '{advice2}'
            Analyze both lists, find the consistent themes, and give me a final, clean list of exactly 3 agreed-upon teaching methods.";

            
            string apiKey = "insertapikey";
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            var package = new { contents = new[] { new { parts = new[] { new { text = masterPrompt } } } } };
            var content = new StringContent(JsonSerializer.Serialize(package), Encoding.UTF8, "application/json");

            var client = new HttpClient();
            var response = await client.PostAsync(url, content);

            string rawJson = await response.Content.ReadAsStringAsync();
            JsonNode jsonNode = JsonNode.Parse(rawJson);

            return jsonNode["candidates"][0]["content"]["parts"][0]["text"].ToString();
        }
    }
}
