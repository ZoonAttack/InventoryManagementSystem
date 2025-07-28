using ProductsManagement.DTOs;
using System.Text;
using System.Text.Json;

namespace ProductsManagement.Models
{
    public class APICalls
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        public APICalls(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["JwtSettings:Key"];
        }


        public async Task<Tuple<string, string>> LoginAsync(LoginUserDto loginDto)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7264/api/Account/Login");
            request.Content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var token = JsonSerializer.Deserialize<LoginResponseDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return Tuple.Create(token.Token, token.Role);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new Tuple<string, string>(null, errorContent);
            }
        }
    }
}
