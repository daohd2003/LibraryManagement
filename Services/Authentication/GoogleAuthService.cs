using LibraryManagement.DTOs.Response;
using System.Text.Json;

namespace LibraryManagement.Services.Authentication
{
    public class GoogleAuthService
    {
        private readonly HttpClient _httpClient;

        public GoogleAuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GooglePayload?> VerifyGoogleTokenAsync(string idToken)
        {
            var response = await _httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={idToken}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var payload = JsonSerializer.Deserialize<GooglePayload>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return payload;
        }
    }
}
