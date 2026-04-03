using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SchoolManagmentSystem.ServicesAi
{
     /*
    public class DeepSeekService : aiRulebook
    {
       
        public async Task<string> GetAdvice(string level, string type, string parentComments, string medicalText)
        {
            string apiKey = "";

            string url = "https://api.deepseek.com/chat/completions";

            string prompt = $"I have a student with {level} and {type} autism. The parent noted: '{parentComments}'. Medical evaluation: '{medicalText}'. Give me exactly 3 short teaching methods.";

            var package = new
            {
                model = "deepseek-chat",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(package), Encoding.UTF8, "application/json");

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await client.PostAsync(url, content);
            string rawJson = await response.Content.ReadAsStringAsync();

            JsonNode jsonNode = JsonNode.Parse(rawJson);

            return jsonNode["choices"][0]["message"]["content"].ToString();
        }
    }*/
}
