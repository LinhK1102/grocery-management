using System.Net.Http.Json;
using System.Text.Json;
using Utility.Common;
using WebApplication.Constants;
using WebApplication.Models;
using WebApplication.Models.Dto;
using WebApplication.Service;

namespace WebApplication.Services
{
    public class OrderDetailApiService : ApiClientService
    {
        public OrderDetailApiService(IHttpClientFactory factory, IHttpContextAccessor accessor, IConfiguration config)
            : base(factory, accessor, config) { }

        public async Task<List<OrderDetailDto>> GetByOrderIdAsync(int orderId)
        {
            var url = string.Format(ApiRoutes.OrderDetails.GetByOrderId, orderId);
            var res = await CreateClient().GetAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<List<OrderDetailDto>>(res);
            return apiResponse.Data ?? new();
        }

        public async Task<bool> CreateAsync(OrderDetailDto dto)
        {
            var res = await CreateClient().PostAsJsonAsync(ApiRoutes.OrderDetails.Create, dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, OrderDetailDto dto)
        {
            var url = string.Format(ApiRoutes.OrderDetails.Update, id);
            var res = await CreateClient().PutAsJsonAsync(url, dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiRoutes.OrderDetails.Delete, id);
            var res = await CreateClient().DeleteAsync(url);
            return res.IsSuccessStatusCode;
        }
    }

}
