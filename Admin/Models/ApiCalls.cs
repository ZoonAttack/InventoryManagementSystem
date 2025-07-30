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
        private readonly string _baseUrl = "https://localhost:7264/"; // Adjust the base URL as needed
        private readonly string _token = string.Empty;

        public ApiCalls(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _token = httpContextAccessor.HttpContext?.Session.GetString("token") ?? "INVALID";
        }

        public async Task<ApiResponse<List<CategorySummaryDto>>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}api/category/categories");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<CategorySummaryDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return ApiResponse<List<CategorySummaryDto>>.Ok(data!,
                                                            "Categories retrieved successfully",
                                                            (int)response.StatusCode);
            }
            else
            {
                // Handle error response
                return ApiResponse<List<CategorySummaryDto>>.Fail(
                    $"Failed to retrieve categories: {response.ReasonPhrase}", (int)response.StatusCode);
            }
        }


        public async Task<ApiResponse<CategoryDetailsDto>> GetCategoryAsync(string name)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}api/category/category/{name}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<CategoryDetailsDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return ApiResponse<CategoryDetailsDto>.Ok(data!,
                                                            "Category retrieved successfully",
                                                            (int)response.StatusCode);
            }
            else
            {
                // Handle error response
                return ApiResponse<CategoryDetailsDto>.Fail(
                    $"Failed to retrieve category: {response.ReasonPhrase}", (int)response.StatusCode);
            }
        }

        public async Task<ApiResponse<CategorySummaryDto>> CreateCategoryAsync(CategorySummaryDto dto)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}api/category/create");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token); 
            request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var statusCode = (int)response.StatusCode;

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var data = JsonSerializer.Deserialize<CategorySummaryDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return ApiResponse<CategorySummaryDto>.Ok(data!, "Category created successfully.", statusCode);
            }
            else
            {
                return ApiResponse<CategorySummaryDto>.Fail($"Failed to create category. Server returned: {json}", statusCode);
            }
        }

        public async Task<ApiResponse<List<ProductSummaryDto>>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}api/product/products");
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

        public async Task<ApiResponse<ProductDetailsDto>> GetProductAsync(int ProductId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}api/product/product/{ProductId}");
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

        public async Task<ApiResponse<ProductDetailsDto>> CreateProductAsync(CreateProductDto dto)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}api/product/create");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
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

        public async Task<ApiResponse<ProductDetailsDto>> UpdateOrderAsync(CreateProductDto dto, int productId)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{_baseUrl}api/product/update/{productId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
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

        public async Task<ApiResponse<string>> DeleteProductAsync(int productId)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseUrl}api/product/delete/{productId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
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
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}api/order/orders");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _httpClient.SendAsync(request);
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
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}api/order/order/{orderId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _httpClient.SendAsync(request);
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

        public async Task<ApiResponse<OrderDetailsDto>> CreateOrderAsync(CreateOrderDto dto)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}api/order/create");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
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
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{_baseUrl}api/order/update/{orderId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
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

        public async Task<ApiResponse<string>> DeleteOrderAsync(int orderId)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseUrl}api/order/delete/{orderId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
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


        public async Task<ApiResponse<string>> LogoutAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}api/Account/Logout");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _httpClient.SendAsync(request);
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
