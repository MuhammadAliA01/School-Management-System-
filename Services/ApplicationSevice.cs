using Azure;
using Azure.AI.DocumentIntelligence;
using OpenAI.Chat;

namespace SchoolManagmentSystem.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly DocumentIntelligenceClient _docClient;
        private readonly ChatClient _chatClient;

        public ApplicationService(IConfiguration config)
        {
            _docClient = new DocumentIntelligenceClient(new Uri(config["Azure:DocIntelEndpoint"]), new AzureKeyCredential(config["Azure:DocIntelKey"]));
            _chatClient = new ChatClient("gpt-4o", config["OpenAI:Key"]);
        }

        public async Task<string> ProcessApplicationAsync(IFormFile idDoc, IFormFile appForm)
        {
            // 1. Extract Text from both documents using Azure
            string idText = await ExtractText(idDoc);
            string appText = await ExtractText(appForm);

            // 2. Build the AI Prompt
            string prompt = $@"
            Compare the following two documents. 
            Document 1 (ID): {idText}
            Document 2 (Application): {appText}
            
            Rules:
            1. If the Full Name and ID Number match exactly, return 'ACCEPTED'.
            2. Otherwise, return 'REJECTED' followed by a short reason.
            Return ONLY the status and reason.";

            // 3. Get Decision from OpenAI
            ChatCompletion completion = await _chatClient.CompleteChatAsync(prompt);
            return completion.Content[0].Text;
        }

        private async Task<string> ExtractText(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            BinaryData data = BinaryData.FromStream(stream);

            // Create options and pass the document data directly
            var options = new AnalyzeDocumentOptions("prebuilt-read", data);

            // Call the method using the new options object
            var operation = await _docClient.AnalyzeDocumentAsync(WaitUntil.Completed, options);

            return operation.Value.Content;
        }
    }
}
