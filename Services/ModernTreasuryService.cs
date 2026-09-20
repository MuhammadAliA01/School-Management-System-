using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SchoolManagmentSystem.Services
{
    public class ModernTreasuryService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ModernTreasuryService(
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<(bool Success, string Message)> TransferAsync(
            string fromAccountId,
            string toAccountId,
            decimal amount,
            string description)
        {
            string organizationId =
                _configuration["ModernTreasury:OrganizationId"];

            string apiKey =
                _configuration["ModernTreasury:ApiKey"];

            if (string.IsNullOrWhiteSpace(organizationId))
            {
                return (false, "Modern Treasury Organization ID is missing.");
            }

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return (false, "Modern Treasury API key is missing.");
            }

            if (string.IsNullOrWhiteSpace(fromAccountId))
            {
                return (false, "Source Modern Treasury account ID is missing.");
            }

            if (string.IsNullOrWhiteSpace(toAccountId))
            {
                return (false, "Destination Modern Treasury account ID is missing.");
            }

            if (amount <= 0)
            {
                return (false, "Transfer amount must be greater than zero.");
            }

            
            long amountInCents = (long)Math.Round(amount * 100);

            var requestBody = new
            {
                description = description,
                effective_at = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                status = "posted",

                ledger_entries = new[]
                {
                    new
                    {
                        amount = amountInCents,
                        direction = "debit",
                        ledger_account_id = fromAccountId
                    },

                    new
                    {
                        amount = amountInCents,
                        direction = "credit",
                        ledger_account_id = toAccountId
                    }
                }
            };

            string json = JsonSerializer.Serialize(requestBody);

            using var request = new HttpRequestMessage(
    HttpMethod.Post,
    "api/ledger_transactions");

            request.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            
            string credentials =
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(
                        $"{organizationId}:{apiKey}"));

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    credentials);

            HttpResponseMessage response =
                await _httpClient.SendAsync(request);

            string responseBody =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return (
                    false,
                    $"Modern Treasury returned {(int)response.StatusCode}: {responseBody}"
                );
            }

            return (
                true,
                responseBody
            );
        }
    }
}