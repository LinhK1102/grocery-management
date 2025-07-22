using GroceryWebApp.Models.Dto;

namespace GroceryWebApp.Service
{
    public class InvoiceItemApiService
    {
        private readonly HttpClient _httpClient;

        public InvoiceItemApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GroceryAPI");
        }

        public async Task<List<InvoiceItemDto>> GetByInvoiceIdAsync(int invoiceId)
        {
            var response = await _httpClient.GetAsync($"/api/invoice-items/by-invoice/{invoiceId}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<InvoiceItemDto>>();
            return result ?? new();
        }
    }
}
