using Utility.Common;
using WebApplication.Constants;
using WebApplication.Models;
using WebApplication.Models.Dto;
using WebApplication.Service;

namespace WebApplication.Services
{
    public class CustomerApiService : ApiClientService
    {
        public CustomerApiService(IHttpClientFactory factory, IHttpContextAccessor accessor, IConfiguration config)
            : base(factory, accessor, config) { }

        public async Task<List<CustomerDto>> SearchCustomersAsync(string searchTerm)
        {
            var client = CreateClient();
            var url = string.Format(ApiRoutes.Customers.Search, searchTerm);
            var res = await client.GetAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<List<CustomerDto>>(res);
            return apiResponse.Data ?? new();
        }

        public async Task<List<CustomerDto>> GetHighDiscountCustomersAsync(decimal minDiscount)
        {
            var client = CreateClient();
            var url = string.Format(ApiRoutes.Customers.HighDiscount, minDiscount);
            var res = await client.GetAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<List<CustomerDto>>(res);
            return apiResponse.Data ?? new();
        }

        public async Task<bool> UpdateDiscountRateAsync(int customerId, decimal newRate, decimal maxLimit)
        {
            var client = CreateClient();
            var url = string.Format(ApiRoutes.Customers.UpdateDiscount, customerId, newRate, maxLimit);
            var res = await client.PostAsync(url, null);
            return res.IsSuccessStatusCode;
        }
    }

}
