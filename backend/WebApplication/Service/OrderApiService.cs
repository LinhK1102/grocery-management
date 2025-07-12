using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Json;
using Utility.Common;
using Utility.Hubs;
using WebApplication.Constants;
using WebApplication.Models;
using WebApplication.Models.Dto;
using WebApplication.Service;

namespace WebApplication.Services
{
    public class OrderApiService : ApiClientService
    {
        public OrderApiService(IHttpClientFactory factory, IHttpContextAccessor accessor, IConfiguration config, IHubContext<NotificationHub> hubContext)
            : base(factory, accessor, config, hubContext) { }

        public async Task<List<OrderDto>> GetAllAsync()
        {
            var res = await CreateClient().GetAsync(ApiRoutes.Orders.GetAll);
            var apiResponse = await JsonUtility.DeserializeWrappedListAsync<OrderDto>(res);
            return apiResponse;
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var url = string.Format(ApiRoutes.Orders.GetById, id);
            var res = await CreateClient().GetAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<OrderDto>(res);
            return apiResponse.Data;
        }

        public async Task<bool> CreateAsync(OrderDto dto)
        {
            var res = await CreateClient().PostAsJsonAsync(ApiRoutes.Orders.Create, dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, OrderDto dto)
        {
            var url = string.Format(ApiRoutes.Orders.Update, id);
            var res = await CreateClient().PutAsJsonAsync(url, dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiRoutes.Orders.Delete, id);
            var res = await CreateClient().DeleteAsync(url);
            return res.IsSuccessStatusCode;
        }

        public async Task<List<OrderDto>> SearchAsync(string keyword)
        {
            var url = string.Format(ApiRoutes.Orders.Search, keyword);
            var res = await CreateClient().GetAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<List<OrderDto>>(res);
            return apiResponse.Data ?? new();
        }
    }
}

