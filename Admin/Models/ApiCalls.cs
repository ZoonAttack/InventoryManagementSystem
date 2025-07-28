using Shared.DTOs;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ProductsManagement.Models
{
    public class ApiCalls
    {
        private readonly HttpClient _httpClient;
        //private string adminToken = string.Empty;
       // private readonly string _apiKey;
        private readonly string _baseUrl = "https://localhost:7264/"; // Adjust the base URL as needed
        public ApiCalls(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            //_apiKey = configuration["JwtSettings:Key"];
        }

        public async Task<List<ProductSummaryDto>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}api/Product/products");
            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<ProductSummaryDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return data ?? new List<ProductSummaryDto>();
            }
            else
            {
                // Handle error response
                return new List<ProductSummaryDto>();
            }

        }

        public async Task<List<OrderSummaryDto>> GetOrdersAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}api/Order/orders");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<OrderSummaryDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return data ?? new List<OrderSummaryDto>();
            }
            else
            {
                // Handle error response
                return new List<OrderSummaryDto>();
            }
        }

        public async Task<Tuple<string, string>> LoginAsync(LoginUserDto loginDto)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}api/Account/Login");
            request.Content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var token = JsonSerializer.Deserialize<LoginResponseDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
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
