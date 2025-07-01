using System.Text.Json;
using WebApplication.Models;

namespace WebApplication.Service
{
    public class ProductApiService : ApiClientService
    {
        public ProductApiService(IHttpClientFactory factory, IHttpContextAccessor contextAccessor, IConfiguration config)
            : base(factory, contextAccessor, config) { }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var client = CreateClient();
            var res = await client.GetAsync("/api/Products/GetAllProduct");
            if (!res.IsSuccessStatusCode) return new List<ProductDto>();

            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProductDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        }
    }

}
