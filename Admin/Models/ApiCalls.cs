using Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using System.Net;
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

        public async Task<ApiResponse<CategoryDto>> CreateOrder(CategoryDto dto)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}api/order/create");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "<Token>"); 
            request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var statusCode = (int)response.StatusCode;

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var data = JsonSerializer.Deserialize<CategoryDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return ApiResponse<CategoryDto>.Ok(data!, "Order created successfully.", statusCode);
            }
            else
            {
                return ApiResponse<CategoryDto>.Fail($"Failed to create order. Server returned: {json}", statusCode);
            }
        }

        public async Task<ApiResponse<List<ProductSummaryDto>>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}api/order/products");
            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<ProductSummaryDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return ApiResponse<List<ProductSummaryDto>>.Ok(data,
                                                                "Products retrieved successfully",
                                                                (int)response.StatusCode);
            }
            else
            {
                // Handle error response
                return ApiResponse<List<ProductSummaryDto>>.Fail(
                    $"Failed to retrieve products: {response.ReasonPhrase}", (int)response.StatusCode);
            }
        }

        public async Task<ApiResponse<ProductDetailsDto>> GetProductAsync(int orderId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}api/order/order/{orderId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ProductDetailsDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return ApiResponse<ProductDetailsDto>.Ok(data!,
                                                        "Product retrieved successfully",
                                                        (int)response.StatusCode);
            }
            else
            {
                // Handle error response
                return ApiResponse<ProductDetailsDto>.Fail(
                    $"Failed to retrieve product: {response.ReasonPhrase}", (int)response.StatusCode);
            }
        }

        public async Task<ApiResponse<ProductDetailsDto>> CreateProduct(CreateProductDto dto)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}api/product/create");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "<Token>");
            request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var statusCode = (int)response.StatusCode;

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var data = JsonSerializer.Deserialize<ProductDetailsDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return ApiResponse<ProductDetailsDto>.Ok(data!, "Order created successfully.", statusCode);
            }
            else
            {
                return ApiResponse<ProductDetailsDto>.Fail($"Failed to create order. Server returned: {json}", statusCode);
            }
        }

        public async Task<ApiResponse<ProductDetailsDto>> UpdateProduct(CreateProductDto dto, int productId)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}api/product/update/{productId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "<Token>");
            request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return ApiResponse<ProductDetailsDto>.Fail(
                    $"Failed to update product: {json}", (int)response.StatusCode);
            }

            var data = JsonSerializer.Deserialize<ProductDetailsDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return ApiResponse<ProductDetailsDto>.Ok(data, "Product updated successfully", (int)response.StatusCode);
        }

        public async Task<ApiResponse<string>> DeleteProduct(int productId)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}api/product/delete/{productId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "<Token>");
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                // Handle error response if needed
                return ApiResponse<string>.Fail("Failed to delete product", (int)response.StatusCode);
            }
            // Optionally, you can handle successful deletion here
            return ApiResponse<string>.Ok("Product deleted successfully", null, (int)response.StatusCode);
        }

        public async Task<ApiResponse<List<OrderSummaryDto>>> GetOrdersAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}api/order/orders");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<OrderSummaryDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return ApiResponse<List<OrderSummaryDto>>.Ok(data,
                                                                "Orders retrieved successfully",
                                                                (int)response.StatusCode);
            }
            else
            {
                // Handle error response
                return ApiResponse<List<OrderSummaryDto>>.Fail(
                    $"Failed to retrieve Orders: {response.ReasonPhrase}", (int)response.StatusCode);
            }

        }

        public async Task<ApiResponse<OrderDetailsDto>> GetOrderAsync(int orderId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}api/order/order/{orderId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<OrderDetailsDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return ApiResponse<OrderDetailsDto>.Ok(data,
                                                        "Order retrieved successfully",
                                                        (int)response.StatusCode);
            }
            else
            {
                // Handle error response
                return ApiResponse<OrderDetailsDto>.Fail(
                    $"Failed to retrieve order: {response.ReasonPhrase}", (int)response.StatusCode);
            }
        }

        public async Task<ApiResponse<OrderDetailsDto>> CreateOrder(CreateOrderDto dto)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}api/order/create");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "<Token>");
            request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var statusCode = (int)response.StatusCode;

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var data = JsonSerializer.Deserialize<OrderDetailsDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return ApiResponse<OrderDetailsDto>.Ok(data!, "Order created successfully.", statusCode);
            }
            else
            {
                return ApiResponse<OrderDetailsDto>.Fail($"Failed to create order. Server returned: {json}", statusCode);
            }
        }

        public async Task<ApiResponse<OrderSummaryDto>> UpdateOrderStatusAsync(int orderId, string status)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}api/order/update/{orderId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "<Token>");
            request.Content = new StringContent(status, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<OrderSummaryDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return ApiResponse<OrderSummaryDto>.Ok(data, "Order status updated successfully", (int)response.StatusCode);
            }
            else
            {
                return ApiResponse<OrderSummaryDto>.Fail(
                    $"Failed to update order status: {response.ReasonPhrase}", (int)response.StatusCode);
            }
        }

        public async Task<ApiResponse<string>> DeleteOrder(int orderId)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}api/order/delete/{orderId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "<Token>");
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                // Handle error response if needed
                return ApiResponse<string>.Fail("Failed to delete product", (int)response.StatusCode);
            }
            // Optionally, you can handle successful deletion here
            return ApiResponse<string>.Ok("Product deleted successfully", null, (int)response.StatusCode);
        }


        public async Task<ApiResponse<Tuple<string, string>>> LoginAsync(LoginUserDto loginDto)
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
                return ApiResponse<Tuple<string, string>>.Ok(
                    new Tuple<string, string>(token.Token, token.Role),
                    "Login successful", (int)response.StatusCode);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResponse<Tuple<string, string>>.Fail(
                    $"Login failed: {errorContent}", (int)response.StatusCode);
            }
        }


        public async Task<ApiResponse<string>> Logout()
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}api/Account/Logout", null);
            if(!response.IsSuccessStatusCode)
            {
                return ApiResponse<string>.Fail(
                    "Failed to log out", (int)response.StatusCode);
            }
            _httpClient.DefaultRequestHeaders.Authorization = null;
            return ApiResponse<string>.Ok(
                "Logged out successfully", null, (int)response.StatusCode);

        }
    }
}
