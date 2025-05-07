using System.Text.Json;

namespace LibraryManagement.DTOs.Response
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public override string ToString() =>
            JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
    }
}
