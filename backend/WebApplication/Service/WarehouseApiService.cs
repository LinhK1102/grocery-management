using System.Text.Json;
using GroceryWebApp.Models;
using Microsoft.AspNetCore.Http;
using GroceryWebApp.Service;
using Utility.Common;
using GroceryWebApp.Constants;
using GroceryWebApp.Helpers;
using GroceryWebApp.Models.Dto;
using Microsoft.AspNetCore.SignalR;
using Utility.Hubs;

namespace GroceryWebApp.Services
{
    public class WarehouseApiService : ApiClientService
    {
        public WarehouseApiService(IHttpClientFactory factory, IHttpContextAccessor accessor, IConfiguration config, IHubContext<NotificationHub> hubContext)
            : base(factory, accessor, config, hubContext) { }

        public async Task<List<WarehouseDto>> GetAllAsync()
        {
            var res = await CreateClient().GetAsync(ApiRoutes.Warehouse.GetAll);
            var apiResponse = await JsonUtility.DeserializeWrappedListAsync<WarehouseDto>(res);
            return apiResponse;
        }

        public async Task<WarehouseDto?> GetByIdAsync(int id)
        {
            var url = string.Format(ApiRoutes.Warehouse.GetById, id);
            var res = await CreateClient().GetAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<WarehouseDto>(res);
            return apiResponse.Data;
        }

        public async Task<bool> CreateAsync(WarehouseDto dto)
        {
            var res = await CreateClient().PostAsJsonAsync(ApiRoutes.Warehouse.Create, dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, WarehouseDto dto)
        {
            var url = string.Format(ApiRoutes.Warehouse.Update, id);
            var res = await CreateClient().PutAsJsonAsync(url, dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiRoutes.Warehouse.Delete, id);
            var res = await CreateClient().DeleteAsync(url);
            return res.IsSuccessStatusCode;
        }

        public async Task<WarehouseDto?> SearchWarehouseName(string warehouseName)
        {
            var url = string.Format(ApiRoutes.Warehouse.Search, warehouseName);
            var res = await CreateClient().GetAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<WarehouseDto>(res);
            return apiResponse.Data;
        }

    }
}
