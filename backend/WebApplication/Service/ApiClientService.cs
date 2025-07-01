using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using WebApplication.Models;

namespace WebApplication.Service
{
    public class ApiClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;

        public ApiClientService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }

        public HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiBaseUrl"]);

            var token = _httpContextAccessor.HttpContext?.User?.FindFirst("Token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        // Ví dụ: Gọi API GetAllProduct
        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var client = CreateClient();
            var response = await client.GetAsync("/api/Products/GetAllProduct");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<ProductDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return products ?? new List<ProductDto>();
            }

            return new List<ProductDto>();
        }
    }
}
