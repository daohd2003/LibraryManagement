using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LibraryManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using LibraryManagement.DTOs;

namespace LibraryManagement.Services.AI
{
    public class AiSearchService : IAiSearchService
    {
        private readonly LibraryDbContext _context;
        private readonly OpenAIOptions _openAIOptions;
        private readonly ILogger<AiSearchService> _logger;
        private readonly HttpClient _httpClient;

        public AiSearchService(
            LibraryDbContext context,
            IOptions<OpenAIOptions> openAIOptions,
            ILogger<AiSearchService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _openAIOptions = openAIOptions.Value;

            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> AskAboutLibraryAsync(string question)
        {
            _logger.LogInformation("Received question: {Question}", question);

            var books = await _context.Books
                .Select(b => $"{b.Title} - {b.Author} ({b.PublicationYear})")
                .ToListAsync();

            var contextString = books.Count > 0
                ? string.Join("\n", books)
                : "Thư viện không có sách nào.";

            var apiKey = _openAIOptions.ApiKey;
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

            var requestData = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = $"Bạn là trợ lý thư viện.\n\nDưới đây là danh sách sách hiện có:\n{contextString}\n\nCâu hỏi của người dùng: {question}"
                            }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Gemini API lỗi: {StatusCode} - {Reason}", response.StatusCode, response.ReasonPhrase);
                return "Xin lỗi, trợ lý không thể trả lời câu hỏi vào lúc này.";
            }

            var responseJson = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseJson);
            var messageContent = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return messageContent ?? "Không có phản hồi.";
        }
    }
}