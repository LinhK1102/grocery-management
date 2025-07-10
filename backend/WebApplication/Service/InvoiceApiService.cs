using WebApplication.Models.Dto;

namespace WebApplication.Service
{
    public class InvoiceApiService
    {
        private readonly HttpClient _httpClient;

        public InvoiceApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GroceryAPI");
        }

        public async Task<List<InvoiceDto>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("/api/invoices/get-all");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<InvoiceDto>>();
            return result ?? new();
        }

        public async Task<InvoiceDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/invoices/get-by-id/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<InvoiceDto>();
            return result;
        }
    }
}
