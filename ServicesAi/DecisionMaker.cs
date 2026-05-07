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
            Analyze both lists and give me a final, tailored list of exactly 3 agreed-upon teaching methods. 

            CRITICAL FORMATTING RULES - YOU MUST OBEY THESE:
            1. DO NOT introduce yourself (Do not say 'As a Coordinator...').
            2. DO NOT use any introductory or concluding sentences (Do not say 'Here is the list...').
            . Start your response immediately with '1. ' and only provide the 3 methods.";

            string apiKey = "";
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            var package = new { contents = new[] { new { parts = new[] { new { text = masterPrompt } } } } };
            var content = new StringContent(JsonSerializer.Serialize(package), Encoding.UTF8, "application/json");

            var client = new HttpClient();
            var response = await client.PostAsync(url, content);

            string rawJson = await response.Content.ReadAsStringAsync();
            /*if (!response.IsSuccessStatusCode)
            {
                return $" MASTER AI (GEMINI) ERROR: {rawJson}";
            }  */
            
            JsonNode jsonNode = JsonNode.Parse(rawJson);

            string finalAnswer = jsonNode["candidates"][0]["content"]["parts"][0]["text"].ToString();
            finalAnswer = finalAnswer.Replace("**", "");
            return finalAnswer;
        }
    }
}
