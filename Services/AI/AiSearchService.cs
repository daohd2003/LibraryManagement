using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LibraryManagement.Data;
using LibraryManagement.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LibraryManagement.Services.AI
{
    public class AiSearchService : IAiSearchService
    {
        private readonly LibraryDbContext _context;
        private readonly OpenAIOptions _openAIOptions;
        private readonly ILogger<AiSearchService> _logger;
        private readonly HttpClient _httpClient;

        public AiSearchService(LibraryDbContext context, IOptions<OpenAIOptions> openAIOptions, ILogger<AiSearchService> logger, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _openAIOptions = openAIOptions.Value;

            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://openrouter.ai");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAIOptions.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://your-app.com"); // bắt buộc
            _httpClient.DefaultRequestHeaders.Add("X-Title", "Library Management AI");
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

            // Tạo payload cho API OpenRouter
            var requestData = new
            {
                model = "deepseek/deepseek-chat-v3-0324:free",
                messages = new[]
                {
                    new { role = "system", content = "Bạn là trợ lý thư viện, bạn có thể giúp trả lời câu hỏi dựa trên dữ liệu sách." },
                    new { role = "system", content = $"Danh sách sách hiện có:\n{contextString}" },
                    new { role = "user", content = question }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/v1/chat/completions", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("AI API lỗi: {StatusCode} - {Reason}", response.StatusCode, response.ReasonPhrase);
                return "Xin lỗi, trợ lý không thể trả lời câu hỏi vào lúc này.";
            }

            var responseJson = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseJson);

            var messageContent = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return messageContent;
        }
    }
}